param(
    [Parameter(Mandatory)][ValidateSet('sandbox', 'local')][string]$Target,
    [int]$AppPid = 0,
    [ValidateSet('sandbox', 'local')][string]$AppScope,
    [string]$ArtifactDirectory = (Join-Path $PSScriptRoot ('artifacts\' + [guid]::NewGuid().ToString('N')))
)

$ErrorActionPreference = 'Stop'
$PSNativeCommandUseErrorActionPreference = $false
$scopeArgs = @(if ($Target -eq 'sandbox') { '--on'; 'sandbox' })
$previousWorkflowId = $env:WINAPP_UI_WORKFLOW_ID
$env:WINAPP_UI_WORKFLOW_ID = [guid]::NewGuid().ToString('N')
$results = [Collections.Generic.List[object]]::new()
$screenshots = [Collections.Generic.List[string]]::new()
$project = Join-Path $PSScriptRoot '..\WinUIGallery.csproj'
$catalog = Get-Content (Join-Path $PSScriptRoot '..\Models\ControlsInfoData.json') -Raw | ConvertFrom-Json
$uiStarted = $false

function Invoke-WinAppChecked {
    param([string[]]$Arguments)
    $global:LASTEXITCODE = $null
    $output = @(& winapp @Arguments)
    if ($null -eq $global:LASTEXITCODE -or $global:LASTEXITCODE -ne 0) {
        throw "winapp $($Arguments -join ' ') failed (exit $global:LASTEXITCODE). $($output -join "`n")"
    }
    $output
}

function Invoke-Ui {
    param([string[]]$Arguments, [string]$Window)
    $selector = if ($Window) { @('-w', $Window) } else { @('-a', "$AppPid") }
    Invoke-WinAppChecked (@('ui') + $Arguments + $scopeArgs + $selector)
}

function Test-UI {
    param([string]$Name, [scriptblock]$Script)
    try {
        & $Script | Out-Null
        $results.Add(@{ name = $Name; status = 'PASS' })
        Write-Host "PASS $Name"
    } catch {
        $results.Add(@{ name = $Name; status = 'FAIL'; detail = "$_" })
        Write-Host "FAIL $Name : $_"
    }
}

function Save-Screenshot {
    param([string]$Name)
    $path = Join-Path $ArtifactDirectory $Name
    if (Test-Path -LiteralPath $path) { throw "Evidence already exists: $path" }
    Invoke-Ui @('screenshot', '--output', $path, '--focus') -Window $hwnd | Out-Null
    if (-not (Test-Path -LiteralPath $path -PathType Leaf) -or (Get-Item -LiteralPath $path).Length -eq 0) {
        throw "Screenshot not delivered: $path"
    }
    $screenshots.Add($path)
}

function Open-Route {
    param([string]$Route)
    $group = $catalog | Where-Object { $_.Items.PageName -contains $Route } | Select-Object -First 1
    if ($group) {
        Invoke-Ui @('invoke', "Nav$($group.PageName)", '--action', 'expand') | Out-Null
    }
    Invoke-Ui @('invoke', "Nav$Route", '--action', 'select') | Out-Null
    $item = @($catalog | ForEach-Object { $_; $_.Items } | Where-Object { $_.PageName -eq $Route })[0]
    $title = if ($Route -eq 'SettingsPage') { 'Settings' } else { $item.Title }
    Invoke-Ui @('wait-for', 'PageTitle', '--value', $title, '-t', '4000') | Out-Null
}

function Select-Theme {
    param([string]$Theme)
    Invoke-Ui @('invoke', 'AppTheme', '--action', 'expand') -Window $hwnd | Out-Null
    $items = Invoke-Ui @('search', $Theme, '--type', 'ListItem', '--json') -Window $hwnd | ConvertFrom-Json
    $item = $items.matches | Where-Object { $_.name -eq $Theme } | Select-Object -First 1
    if (-not $item) { throw "Theme item not found: $Theme" }
    Invoke-Ui @('invoke', $item.selector, '--action', 'select') -Window $hwnd | Out-Null
}

try {
    $ArtifactDirectory = [IO.Path]::GetFullPath($ArtifactDirectory)
    New-Item -ItemType Directory -Force -Path $ArtifactDirectory | Out-Null
    if ($AppPid -gt 0) {
        if ($AppScope -ne $Target) { throw 'Reusing a PID requires matching AppScope and Target.' }
    } elseif ($AppPid -eq 0) {
        $launch = Invoke-WinAppChecked (@('run', $project, '--arch', 'x64', '--detach', '--json') + $scopeArgs) | ConvertFrom-Json
        if ($launch.Error -or -not $launch.ProcessId) { throw "No app ProcessId: $($launch.Error)" }
        $AppPid = [int]$launch.ProcessId
        if ($Target -eq 'sandbox' -and
            ($launch.ProcessScope -ne 'sandbox' -or $launch.UiTargetArgs -ne "--on sandbox -a $AppPid")) {
            throw 'Invalid Sandbox process scope.'
        }
    } else {
        throw 'AppPid must be positive, or zero to launch.'
    }
    $uiStarted = $true
    $windows = @(Invoke-Ui @('list-windows', '--json') | ConvertFrom-Json)
    $main = $windows | Where-Object { $_.hwnd -and $_.title -eq 'Gallery (WinUI 3)' } | Select-Object -First 1
    if (-not $main) { throw 'No live Gallery window found in the selected target.' }
    $hwnd = [string]$main.hwnd

    Test-UI 'Home and initial screenshot' {
        Open-Route 'DashboardPage'
        Save-Screenshot '01-home.png'
    }
    foreach ($group in $catalog) {
        Test-UI "Route $($group.PageName)" { Open-Route $group.PageName }
        foreach ($sample in $group.Items) {
            Test-UI "Route $($sample.PageName)" { Open-Route $sample.PageName }
        }
    }
    Test-UI 'Button click updates result' {
        Open-Route 'ButtonPage'
        Invoke-Ui @('invoke', 'DemoButton')
        Invoke-Ui @('wait-for', 'ButtonResult', '--value', 'Clicked 1 time(s).')
        Save-Screenshot '02-button.png'
    }
    Test-UI 'Select-all updates child CheckBoxes' {
        Open-Route 'CheckBoxPage'
        Invoke-Ui @('invoke', 'SelectAll', '--action', 'toggle-on')
        Invoke-Ui @('wait-for', 'OptionOne', '--value', 'On')
        Invoke-Ui @('invoke', 'OptionOne', '--action', 'toggle-off')
        Invoke-Ui @('wait-for', 'SelectAll', '--value', 'Indeterminate')
    }
    Test-UI 'Invalid and valid text' {
        Open-Route 'TextBoxPage'
        Invoke-Ui @('set-value', 'ValidatedText', '123')
        Invoke-Ui @('wait-for', 'ValidationError')
        Invoke-Ui @('set-value', 'ValidatedText', 'Gallery')
        Invoke-Ui @('wait-for', 'ValidationError', '--gone')
    }
    Test-UI 'Frame navigation' {
        Open-Route 'FramePage'
        Invoke-Ui @('wait-for', 'FramePageNumber', '--value', 'Page 1')
        Invoke-Ui @('invoke', 'FrameNext')
        Invoke-Ui @('wait-for', 'FramePageNumber', '--value', 'Page 2')
    }
    Test-UI 'List selection after attaching the sample namescope' {
        Open-Route 'ListViewPage'
        $matches = Invoke-Ui @('search', 'John Doe', '--root', 'DemoPeople', '--type', 'ListItem', '--json') |
            ConvertFrom-Json
        if (-not $matches.matches.Count) { throw 'No sample person found.' }
        Invoke-Ui @('invoke', $matches.matches[0].selector, '--action', 'select')
        Invoke-Ui @('wait-for', 'ListSelectionResult', '--value', '1 selected')
    }
    Test-UI 'Window resize after attaching the sample namescope' {
        Open-Route 'ResizeGripPage'
        Invoke-Ui @('invoke', 'ResizeWindow')
        Invoke-Ui @('wait-for', 'ResizeResult', '--value', 'Window resized to 1200 x 800 DIPs.')
    }
    Test-UI 'Icon pagination, filtering, and empty result' {
        Open-Route 'IconsPage'
        Invoke-Ui @('invoke', 'NextIconPage')
        Invoke-Ui @('wait-for', 'IconPageSummary', '--value', 'Page 2 of', '--contains')
        Invoke-Ui @('set-value', 'IconSearch', 'no-such-gallery-icon-xyz')
        Invoke-Ui @('wait-for', 'IconPageSummary', '--value', '0 icons - Page 1 of 1')
        Invoke-Ui @('set-value', 'IconSearch', 'home')
        Invoke-Ui @('wait-for', 'IconPageSummary', '--value', 'Page 1 of', '--contains')
        Save-Screenshot '03-icons.png'
    }
    Test-UI 'ContentDialog result' {
        Open-Route 'MessageBoxPage'
        Invoke-Ui @('invoke', 'ShowSimpleDialog')
        Invoke-Ui @('wait-for', 'PrimaryButton')
        Invoke-Ui @('invoke', 'PrimaryButton')
        Invoke-Ui @('wait-for', 'DialogResult', '--value', 'Result: OK')
    }
    Test-UI 'Dashboard edit, validation, cancel, add, save, and delete confirmation' {
        Open-Route 'UserDashboardPage'
        Invoke-Ui @('invoke', 'EditUser')
        Invoke-Ui @('set-value', 'UserFirstName', 'GalleryTest')
        Invoke-Ui @('invoke', 'SaveUser')
        Invoke-Ui @('wait-for', 'UserStatus', '--value', 'Saved GalleryTest Doe.')
        Invoke-Ui @('invoke', 'EditUser')
        Invoke-Ui @('set-value', 'UserFirstName', ' ')
        Invoke-Ui @('invoke', 'SaveUser')
        Invoke-Ui @('wait-for', 'UserStatus', '--value', 'Enter a first name', '--contains')
        Invoke-Ui @('invoke', 'CancelUser')
        Invoke-Ui @('wait-for', 'UserFirstName', '--value', 'GalleryTest')
        Invoke-Ui @('invoke', 'AddUser')
        Invoke-Ui @('set-value', 'UserFirstName', 'Added')
        Invoke-Ui @('set-value', 'UserLastName', 'Person')
        Invoke-Ui @('invoke', 'SaveUser')
        Invoke-Ui @('wait-for', 'UserStatus', '--value', 'Saved Added Person.')
        Invoke-Ui @('invoke', 'DeleteUser')
        Invoke-Ui @('wait-for', 'PrimaryButton')
        Invoke-Ui @('invoke', 'CloseButton')
        Invoke-Ui @('wait-for', 'UserFirstName', '--value', 'Added')
        Invoke-Ui @('invoke', 'DeleteUser')
        Invoke-Ui @('wait-for', 'PrimaryButton')
        Invoke-Ui @('invoke', 'PrimaryButton')
        Invoke-Ui @('wait-for', 'UserStatus', '--value', 'Deleted Added Person.')
        Save-Screenshot '04-dashboard.png'
    }
    Test-UI 'Settings and live Light/Dark themes' {
        Open-Route 'SettingsPage'
        Select-Theme 'Dark'
        Invoke-Ui @('wait-for', 'AppTheme', '--value', 'Dark')
        Save-Screenshot '05-dark.png'
        Select-Theme 'Light'
        Invoke-Ui @('wait-for', 'AppTheme', '--value', 'Light')
        Save-Screenshot '06-light.png'
        Select-Theme 'Use system setting'
    }
    Test-UI 'File and folder picker cancellation' {
        Open-Route 'FileAndFolderDialogsPage'
        foreach ($case in @(
            @{ button = 'PickSingleFile'; title = 'Select a file'; expected = 'File selection cancelled.' },
            @{ button = 'PickMultipleFiles'; title = 'Select multiple files'; expected = 'File selection cancelled.' },
            @{ button = 'SaveFile'; title = 'Save file'; expected = 'Save cancelled.' },
            @{ button = 'PickFolder'; title = 'Select a folder'; expected = 'Folder selection cancelled.' }
        )) {
            Invoke-Ui @('invoke', $case.button) | Out-Null
            $picker = $null
            for ($attempt = 0; $attempt -lt 20 -and -not $picker; $attempt++) {
                $windows = @(Invoke-WinAppChecked (@('ui', 'list-windows', '--json') + $scopeArgs) | ConvertFrom-Json)
                $picker = $windows | Where-Object {
                    [string]$_.ownerHwnd -eq $hwnd -and $_.title -eq $case.title
                } | Select-Object -First 1
                if (-not $picker) { Start-Sleep -Milliseconds 250 }
            }
            if (-not $picker) { throw "Owned picker did not open: $($case.title)" }
            Invoke-Ui @('invoke', 'Cancel') -Window ([string]$picker.hwnd) | Out-Null
            Invoke-Ui @('wait-for', 'PickerResult', '--value', $case.expected) | Out-Null
        }
    }
    Test-UI 'Navigation back stack' {
        Open-Route 'ButtonPage'
        Open-Route 'SettingsPage'
        Invoke-Ui @('invoke', 'NavigationViewBackButton')
        Invoke-Ui @('wait-for', 'PageTitle', '--value', 'Button')
    }
    Test-UI 'Final home screenshot' {
        Open-Route 'DashboardPage'
        Save-Screenshot '07-final-home.png'
    }
} catch {
    $results.Add(@{ name = 'Target setup / execution'; status = 'FAIL'; detail = "$_" })
} finally {
    try {
        if ($uiStarted) { Invoke-WinAppChecked (@('ui', 'yield') + $scopeArgs) | Out-Null }
    } catch {
        $results.Add(@{ name = 'Release UI workflow'; status = 'FAIL'; detail = "$_" })
    } finally {
        $env:WINAPP_UI_WORKFLOW_ID = $previousWorkflowId
    }
}

$failed = @($results | Where-Object { $_.status -eq 'FAIL' }).Count
$report = [ordered]@{
    target = $Target; appPid = $AppPid; passed = $results.Count - $failed; failed = $failed
    results = @($results.ToArray()); screenshots = @($screenshots.ToArray())
}
$reportPath = Join-Path $ArtifactDirectory 'test-results.json'
$report | ConvertTo-Json -Depth 6 | Set-Content -LiteralPath $reportPath -Encoding utf8
Write-Host "Passed: $($report.passed) | Failed: $failed | Results: $reportPath"
if ($failed) { exit 1 }
