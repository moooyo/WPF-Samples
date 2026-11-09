#Requires -Version 7.3
param(
    [Parameter(Mandatory)][ValidateSet('sandbox', 'local')][string]$Target,
    [int]$AppPid = 0,
    [ValidateSet('sandbox', 'local')][string]$AppScope,
    [string]$WorkflowId = [guid]::NewGuid().ToString('N'),
    [string]$ArtifactDirectory = (Join-Path $PSScriptRoot "obj\ui-test-artifacts\$([guid]::NewGuid().ToString('N'))")
)

$ErrorActionPreference = 'Stop'
$PSNativeCommandUseErrorActionPreference = $false
$PSNativeCommandArgumentPassing = 'Standard'
$scopeArgs = @(if ($Target -eq 'sandbox') { '--on'; 'sandbox' })
$previousWorkflowId = $env:WINAPP_UI_WORKFLOW_ID
$env:WINAPP_UI_WORKFLOW_ID = $WorkflowId
$results = [Collections.Generic.List[object]]::new()
$screenshots = [Collections.Generic.List[string]]::new()
$ownsApp = $false
$uiStarted = $false
$hwnd = $null
$clipboardBackup = $null
$pageParents = @{}

function Invoke-WinAppChecked {
    param([string[]]$Arguments)
    $global:LASTEXITCODE = $null
    $output = @(& winapp @Arguments)
    $code = $global:LASTEXITCODE
    if ($null -eq $code -or $code -ne 0) {
        throw "winapp $($Arguments -join ' ') failed (exit $code). $($output -join "`n")"
    }
    $output
}

function Invoke-Ui {
    param([string[]]$Arguments, [string]$Window = $hwnd)
    Invoke-WinAppChecked (@('ui') + $Arguments + $scopeArgs + @('-w', $Window))
}

function Test-UI {
    param([string]$Name, [scriptblock]$Script)
    try {
        & $Script | Out-Null
        $results.Add(@{ name = $Name; status = 'PASS' })
        Write-Host "PASS: $Name"
    } catch {
        $results.Add(@{ name = $Name; status = 'FAIL'; detail = "$_" })
        Write-Host "FAIL: $Name - $_"
    }
}

function Get-Elements($Nodes) {
    foreach ($node in $Nodes) {
        if ($null -ne $node) { $node; Get-Elements $node.children }
    }
}

function Read-Tree {
    $tree = Invoke-Ui @('inspect', '--json', '-d', '20') | ConvertFrom-Json
    $elements = @(Get-Elements @($tree.windows | ForEach-Object { $_.elements }))
    if ($elements.Count -lt 20) { throw 'No usable application UI tree was returned.' }
    $elements
}

function Save-Screenshot {
    param([string]$Name, [string]$Window = $hwnd)
    $path = Join-Path $ArtifactDirectory $Name
    if (Test-Path -LiteralPath $path) { throw "Evidence path already exists: $path" }
    Invoke-Ui @('screenshot', '--output', $path) -Window $Window | Out-Null
    if (-not (Test-Path -LiteralPath $path -PathType Leaf) -or (Get-Item -LiteralPath $path).Length -eq 0) {
        throw "Screenshot was not delivered: $path"
    }
    $screenshots.Add($path)
}

function Navigate-Page {
    param([string]$Page, [string]$Title)
    if ($pageParents.ContainsKey($Page)) {
        Invoke-Ui @('invoke', "Nav$($pageParents[$Page])", '--action', 'expand') | Out-Null
    }
    Invoke-Ui @('invoke', "Nav$Page", '--action', 'select') | Out-Null
    Invoke-Ui @('wait-for', "Nav$Page", '-p', 'IsSelected', '--value', 'True', '-t', '3000') | Out-Null
    $elements = @(Read-Tree)
    if ($Page -eq 'UserDashboardPage') {
        Invoke-Ui @('wait-for', 'UserList', '-t', '3000') | Out-Null
    } else {
        if ($Page -eq 'DashboardPage') { $Title = 'WPF Gallery' }
        if ($Page -eq 'WhatsNewPage') { $Title = "What's new in this Gallery" }
        if ($Page -eq 'NavigationWindowPage') { $Title = 'Navigation Window' }
        Invoke-Ui @('wait-for', 'PageTitle', '--value', $Title, '-t', '3000') | Out-Null
    }
}

function Select-Combo {
    param([string]$Selector, [string]$Value)
    Invoke-Ui @('invoke', $Selector) | Out-Null
    $choice = @(Read-Tree | Where-Object { $_.type -eq 'ListItem' -and $_.name -eq $Value }) | Select-Object -First 1
    if (-not $choice.selector) { throw "ComboBox choice '$Value' was not found." }
    Invoke-Ui @('invoke', $choice.selector, '--action', 'select') | Out-Null
    Invoke-Ui @('wait-for', $Selector, '--value', $Value, '-t', '2000') | Out-Null
}

function Get-OwnedPicker {
    for ($attempt = 0; $attempt -lt 20; $attempt++) {
        $windows = @(Invoke-WinAppChecked (@('ui', 'list-windows', '--json') + $scopeArgs) | ConvertFrom-Json)
        $owned = @($windows | Where-Object {
            $_.hwnd -and [string]$_.ownerHwnd -eq $hwnd -and $_.className -eq '#32770'
        })
        if ($owned.Count -eq 1) { return [string]$owned[0].hwnd }
        Start-Sleep -Milliseconds 200
    }
    throw 'No uniquely owned picker window appeared.'
}

try {
    $ArtifactDirectory = [IO.Path]::GetFullPath($ArtifactDirectory)
    New-Item -ItemType Directory -Force -Path $ArtifactDirectory | Out-Null
    if ($AppPid -lt 0) { throw 'AppPid must be positive, or zero to launch.' }
    if ($AppPid -gt 0) {
        if ($AppScope -ne $Target) { throw 'Reusing a PID requires matching AppScope and Target.' }
    } else {
        $launch = Invoke-WinAppChecked (@('run', (Join-Path $PSScriptRoot 'WPFGallery.csproj'),
            '--no-build', '--detach', '--json') + $scopeArgs) | ConvertFrom-Json
        if ($launch.Error -or -not $launch.ProcessId -or [int]$launch.ProcessId -le 0) {
            throw "Gallery launch did not return a process: $($launch.Error)"
        }
        $AppPid = [int]$launch.ProcessId
        $ownsApp = $true
        if ($Target -eq 'sandbox' -and
            ($launch.ProcessScope -ne 'sandbox' -or $launch.UiTargetArgs -ne "--on sandbox -a $AppPid")) {
            throw 'Sandbox launch returned an inconsistent PID / scope pair.'
        }
        if ($Target -eq 'local' -and $launch.ProcessScope -and $launch.ProcessScope -ne 'local') {
            throw 'Launch belongs to another target.'
        }
    }
    $uiStarted = $true
    $main = $null
    for ($attempt = 0; $attempt -lt 40 -and -not $main; $attempt++) {
        $windows = @(Invoke-WinAppChecked (@('ui', 'list-windows', '--json') + $scopeArgs + @('-a', "$AppPid")) | ConvertFrom-Json)
        $main = $windows | Where-Object { $_.hwnd -and $_.title -eq 'WPF Gallery' } | Select-Object -First 1
        if (-not $main) { Start-Sleep -Milliseconds 250 }
    }
    if (-not $main) { throw 'Gallery main window was not found.' }
    $hwnd = [string]$main.hwnd
    Test-UI 'Home window and visible content' {
        Read-Tree
        Invoke-Ui @('wait-for', 'NavDashboardPage', '-t', '3000')
        Invoke-Ui @('wait-for', 'PageTitle', '--value', 'WPF Gallery', '-t', '3000')
        Save-Screenshot '01-home.png'
    }
    $catalog = Get-Content -LiteralPath (Join-Path $PSScriptRoot 'Models\ControlsInfoData.json') -Raw | ConvertFrom-Json
    foreach ($group in $catalog) {
        foreach ($child in $group.Items) { $pageParents[$child.PageName] = $group.PageName }
    }
    foreach ($group in $catalog) {
        $entry = $group
        Test-UI "Navigate $($entry.PageName)" { Navigate-Page $entry.PageName $entry.Title }
        if ($group.Items.Count -gt 0) {
            Test-UI "Expand $($entry.PageName)" { Invoke-Ui @('invoke', "Nav$($entry.PageName)", '--action', 'expand') }
            foreach ($child in $group.Items) {
                $entry = $child
                Test-UI "Navigate $($entry.PageName)" { Navigate-Page $entry.PageName $entry.Title }
            }
            Test-UI "Collapse $($group.PageName)" { Invoke-Ui @('invoke', "Nav$($group.PageName)", '--action', 'collapse') }
        }
    }
    Test-UI 'Settings and theme default' {
        Invoke-Ui @('invoke', 'NavSettings', '--action', 'select')
        Invoke-Ui @('wait-for', 'Change_ThemeMode', '--value', 'Use system setting', '-t', '3000')
    }
    Test-UI 'Dark theme survives navigation' {
        Select-Combo 'Change_ThemeMode' 'Dark'
        Save-Screenshot '02-settings-dark.png'
        Navigate-Page 'DashboardPage' 'WPF Gallery'
        Save-Screenshot '03-home-dark.png'
        Invoke-Ui @('invoke', 'NavSettings', '--action', 'select')
        Invoke-Ui @('wait-for', 'Change_ThemeMode', '--value', 'Dark', '-t', '3000')
        Select-Combo 'Change_ThemeMode' 'Light'
    }
    Test-UI 'Text validation rejects invalid input and recovers' {
        Navigate-Page 'TextBoxPage' 'TextBox'
        Invoke-Ui @('set-value', 'ValidatedTextBox', '123')
        Invoke-Ui @('wait-for', 'TextValidationError', '--value', 'Only English alphabetic', '--contains')
        Save-Screenshot '04-text-validation.png'
        Invoke-Ui @('set-value', 'ValidatedTextBox', 'Alphabetic')
        Invoke-Ui @('wait-for', 'TextValidationError', '--value', '')
    }
    Test-UI 'Editable table validation and descending sort' {
        Navigate-Page 'DataGridPage' 'DataGrid'
        Invoke-Ui @('wait-for', 'ProductID0', '--value', '0')
        Invoke-Ui @('set-value', 'ProductID0', 'invalid')
        Invoke-Ui @('wait-for', 'ProductValidation0', '--value', 'Product ID must be a whole number.')
        Save-Screenshot '11-table-validation.png'
        Invoke-Ui @('set-value', 'ProductID0', '123')
        Invoke-Ui @('wait-for', 'ProductValidation0', '--value', '')
        Invoke-Ui @('invoke', 'SortProductId')
        Invoke-Ui @('invoke', 'SortProductId')
        Invoke-Ui @('wait-for', 'ProductID123', '--value', '123')
        $ids = @(Read-Tree | Where-Object { $_.type -eq 'Edit' -and $_.automationId -match '^ProductID\d+$' -and -not $_.isOffscreen })
        if (-not $ids.Count -or $ids[0].automationId -ne 'ProductID123') {
            throw 'Descending sort did not place the edited product first.'
        }
        Save-Screenshot '12-table-sorted.png'
    }
    Test-UI 'Icons load and paginate' {
        Navigate-Page 'IconsPage' 'Icons'
        Invoke-Ui @('wait-for', 'IconsCurrentPage', '--value', '1')
        Invoke-Ui @('wait-for', 'IconsNextPage', '-p', 'IsEnabled', '--value', 'True')
        Invoke-Ui @('invoke', 'IconsNextPage')
        Invoke-Ui @('wait-for', 'IconsCurrentPage', '--value', '2')
        Invoke-Ui @('invoke', 'IconsPreviousPage')
        Invoke-Ui @('wait-for', 'IconsCurrentPage', '--value', '1')
    }
    Test-UI 'Icon search and empty results' {
        Invoke-Ui @('set-value', 'IconsSearchBox', 'Search')
        Invoke-Ui @('wait-for', 'SelectedIconName')
        $selected = Invoke-Ui @('get-value', 'SelectedIconName', '--json') | ConvertFrom-Json
        $icons = Get-Content -LiteralPath (Join-Path $PSScriptRoot 'Models\IconsData.json') -Raw | ConvertFrom-Json
        $matching = @($icons | Where-Object { $_.Name -match 'Search' -or $_.Tags -match 'Search' })
        if ($selected.text -notin $matching.Name) { throw 'Selected icon does not match the name or tag filter.' }
        Save-Screenshot '05-icons-filtered.png'
        Invoke-Ui @('set-value', 'IconsSearchBox', 'GalleryNoMatchingIcon0123456789')
        Invoke-Ui @('wait-for', 'IconsNextPage', '-p', 'IsEnabled', '--value', 'False')
        $tree = @(Read-Tree)
        if ($tree | Where-Object { $_.type -eq 'ListItem' -and $_.className -eq 'GridViewItem' }) {
            throw 'Empty search still displays icon items.'
        }
        Invoke-Ui @('set-value', 'IconsSearchBox', '')
    }
    Test-UI 'Dashboard add, save, and cancel edits' {
        Navigate-Page 'UserDashboardPage' 'User Dashboard'
        Invoke-Ui @('invoke', 'NewUserButton')
        Invoke-Ui @('wait-for', 'save_button')
        Invoke-Ui @('set-value', 'FirstName', 'GalleryTestUser')
        Invoke-Ui @('scroll-into-view', 'save_button')
        Save-Screenshot '06-user-edit.png'
        Invoke-Ui @('invoke', 'save_button')
        Invoke-Ui @('wait-for', 'UserDetailHeaderNameBox', '--value', 'GalleryTestUser', '--contains')
        Invoke-Ui @('invoke', 'edit_button')
        Invoke-Ui @('set-value', 'FirstName', 'DiscardedEdit')
        Invoke-Ui @('scroll-into-view', 'cancel_button')
        Invoke-Ui @('invoke', 'cancel_button')
        Invoke-Ui @('wait-for', 'FirstName', '--value', 'GalleryTestUser')
    }
    Test-UI 'Dashboard deletion defaults to Cancel' {
        Invoke-Ui @('invoke', 'delete_button')
        Invoke-Ui @('wait-for', 'Delete user')
        Save-Screenshot '07-delete-confirmation.png'
        Invoke-Ui @('send-keys', 'enter', '--via', 'send-input')
        Invoke-Ui @('wait-for', 'Delete user', '--gone')
        Invoke-Ui @('wait-for', 'FirstName', '--value', 'GalleryTestUser')
    }
    Test-UI 'Dashboard confirmed deletion' {
        Invoke-Ui @('invoke', 'delete_button')
        Invoke-Ui @('wait-for', 'Primary')
        Invoke-Ui @('invoke', 'Primary')
        Invoke-Ui @('wait-for', 'Primary', '--gone')
        $tree = @(Read-Tree)
        if ($tree | Where-Object { $_.type -eq 'ListItem' -and $_.name -match 'GalleryTestUser' }) {
            throw 'Confirmed deletion did not remove the added user.'
        }
    }
    Test-UI 'Dialog default No and explicit primary result' {
        Navigate-Page 'MessageBoxPage' 'MessageBox'
        Invoke-Ui @('scroll-into-view', 'ShowCustomDefaultButton_Click')
        Invoke-Ui @('invoke', 'ShowCustomDefaultButton_Click')
        Invoke-Ui @('wait-for', 'Secondary')
        Save-Screenshot '08-dialog-default-no.png'
        Invoke-Ui @('send-keys', 'enter', '--via', 'send-input')
        Invoke-Ui @('wait-for', 'Secondary', '--gone')
        Invoke-Ui @('wait-for', 'User selected: No', '--value', 'User selected: No')
        Invoke-Ui @('invoke', 'ShowDefaultMessageButton_Click')
        Invoke-Ui @('wait-for', 'Primary')
        Invoke-Ui @('invoke', 'Primary')
        Invoke-Ui @('wait-for', 'Primary', '--gone')
        Invoke-Ui @('wait-for', 'Result: OK', '--value', 'Result: OK')
    }
    foreach ($case in @(
        @{ choice = 'OK'; result = 'OK' },
        @{ choice = 'OK/Cancel'; result = 'OK' },
        @{ choice = 'Abort/Retry/Ignore'; result = 'Abort' },
        @{ choice = 'Yes/No/Cancel'; result = 'Yes' },
        @{ choice = 'Yes/No'; result = 'Yes' },
        @{ choice = 'Retry/Cancel'; result = 'Retry' },
        @{ choice = 'Cancel/Try/Continue'; result = 'Cancel' }
    )) {
        Test-UI "Dialog buttons and default: $($case.choice)" {
            Invoke-Ui @('scroll-into-view', 'ButtonTypeComboBox')
            Select-Combo 'ButtonTypeComboBox' $case.choice
            Invoke-Ui @('invoke', 'MessageBox with Different Buttons')
            Invoke-Ui @('wait-for', 'Primary')
            Invoke-Ui @('send-keys', 'enter', '--via', 'send-input')
            Invoke-Ui @('wait-for', 'Primary', '--gone')
            Invoke-Ui @('wait-for', 'DialogButtonsResult', '--value', "Result: $($case.result)")
        }
    }
    Test-UI 'Yes/No cannot silently approve on Escape' {
        Select-Combo 'ButtonTypeComboBox' 'Yes/No'
        Invoke-Ui @('invoke', 'MessageBox with Different Buttons')
        Invoke-Ui @('wait-for', 'Secondary')
        Invoke-Ui @('send-keys', 'escape', '--via', 'send-input')
        Invoke-Ui @('wait-for', 'Secondary')
        Invoke-Ui @('invoke', 'Secondary')
        Invoke-Ui @('wait-for', 'Secondary', '--gone')
        Invoke-Ui @('wait-for', 'DialogButtonsResult', '--value', 'Result: No')
    }
    if ($Target -eq 'local') {
        Test-UI 'Preserve clipboard for local tests' {
            Add-Type -AssemblyName System.Windows.Forms
            $original = [Windows.Forms.Clipboard]::GetDataObject()
            $backup = [Windows.Forms.DataObject]::new()
            if ($original) {
                foreach ($format in $original.GetFormats($false)) {
                    $backup.SetData($format, $false, $original.GetData($format, $false))
                }
                $script:clipboardBackup = $backup
            }
        }
        if ($clipboardBackup) {
            Test-UI 'Clipboard text and image round trips' {
                Navigate-Page 'ClipboardPage' 'Clipboard'
                Invoke-Ui @('set-value', 'CopyTextBox', 'WinUI Gallery clipboard test')
                Invoke-Ui @('invoke', 'CopyToClipboard_Click')
                Invoke-Ui @('invoke', 'PasteFromClipboard_Click')
                Invoke-Ui @('wait-for', 'PasteTextBox', '--value', 'WinUI Gallery clipboard test')
                Invoke-Ui @('scroll-into-view', 'CopyImageToClipboard_Click')
                Invoke-Ui @('invoke', 'CopyImageToClipboard_Click')
                Invoke-Ui @('invoke', 'PasteImageFromClipboard_Click')
                Invoke-Ui @('wait-for', 'Image pasted! Size:', '--value', 'Image pasted! Size:', '--contains')
                Save-Screenshot '09-clipboard-image.png'
            }
        }
    }
    Test-UI 'Picker cancellation preserves existing state' {
        Navigate-Page 'FileAndFolderDialogsPage' 'File and Folder Dialogs'
        foreach ($button in @('PickSingleFileButton_Click', 'PickMultipleFilesButton_Click', 'PickFolderButton_Click')) {
            Invoke-Ui @('scroll-into-view', $button)
            Invoke-Ui @('invoke', $button)
            $picker = Get-OwnedPicker
            Invoke-Ui @('invoke', 'Cancel') -Window $picker
            Invoke-Ui @('wait-for', $button, '-t', '3000')
        }
    }
    if ($Target -eq 'local') {
        Test-UI 'Save picker writes edited content to an owned artifact' {
            $savedFile = Join-Path $ArtifactDirectory 'gallery-save-test.txt'
            if (Test-Path -LiteralPath $savedFile) { throw "Save evidence already exists: $savedFile" }
            $content = 'WinUI Gallery save-file round trip'
            Invoke-Ui @('scroll-into-view', 'Save File Text Box')
            Invoke-Ui @('set-value', 'Save File Text Box', $content)
            Invoke-Ui @('invoke', 'SaveFileButton_Click')
            $picker = Get-OwnedPicker
            Invoke-Ui @('set-value', 'FileNameControlHost', $savedFile) -Window $picker
            Invoke-Ui @('invoke', 'Save') -Window $picker
            Invoke-Ui @('wait-for', 'SavedFileStatus', '--value', "File saved successfully: $savedFile")
            if (-not (Test-Path -LiteralPath $savedFile -PathType Leaf) -or
                [IO.File]::ReadAllText($savedFile) -ne $content) {
                throw 'The saved file did not contain the edited text.'
            }
        }
    }
    Test-UI 'Sample Frame back and forward navigation' {
        Navigate-Page 'FramePage' 'Frame'
        Invoke-Ui @('invoke', 'OpenFrameWindow')
        $sample = $null
        for ($attempt = 0; $attempt -lt 20 -and -not $sample; $attempt++) {
            $windows = @(Invoke-WinAppChecked (@('ui', 'list-windows', '--json') + $scopeArgs +
                @('-a', "$AppPid")) | ConvertFrom-Json)
            $sample = $windows | Where-Object { $_.hwnd -and $_.title -eq 'FrameWindow' } |
                Select-Object -First 1
            if (-not $sample) { Start-Sleep -Milliseconds 200 }
        }
        if (-not $sample) { throw 'The sample Frame window was not created by the Gallery process.' }
        $script:sampleHwnd = [string]$sample.hwnd
        Invoke-Ui @('wait-for', 'This is Page 1') -Window $sampleHwnd
        Invoke-Ui @('invoke', 'This is the link to Page 2') -Window $sampleHwnd
        Invoke-Ui @('wait-for', 'This is Page 2') -Window $sampleHwnd
        Invoke-Ui @('invoke', 'Back_Click') -Window $sampleHwnd
        Invoke-Ui @('wait-for', 'This is Page 1') -Window $sampleHwnd
        Invoke-Ui @('invoke', 'Forward_Click') -Window $sampleHwnd
        Invoke-Ui @('wait-for', 'This is Page 2') -Window $sampleHwnd
        Save-Screenshot '13-frame-history.png' -Window $sampleHwnd
    }
    Test-UI 'Return to system theme and verify navigation back' {
        Invoke-Ui @('invoke', 'NavSettings', '--action', 'select')
        Select-Combo 'Change_ThemeMode' 'Use system setting'
        Navigate-Page 'WhatsNewPage' "What's new in this Gallery"
        Save-Screenshot '10-whats-new.png'
        Invoke-Ui @('invoke', 'NavigationViewBackButton')
        Invoke-Ui @('wait-for', 'Change_ThemeMode', '--value', 'Use system setting')
    }
    if ($ownsApp -and $Target -eq 'local') {
        Test-UI 'Closing the main window also closes tracked sample windows' {
            Invoke-Ui @('invoke', 'Close')
            for ($attempt = 0; $attempt -lt 40; $attempt++) {
                if (-not (Get-Process -Id $AppPid -ErrorAction SilentlyContinue)) { break }
                Start-Sleep -Milliseconds 250
            }
            if (Get-Process -Id $AppPid -ErrorAction SilentlyContinue) {
                throw 'The Gallery process remained alive after closing the main window.'
            }
        }
    }
} catch {
    $results.Add(@{ name = 'Target setup / test execution'; status = 'FAIL'; detail = "$_" })
} finally {
    try {
        if ($clipboardBackup) {
            [Windows.Forms.Clipboard]::SetDataObject($clipboardBackup, $true)
        }
    } catch {
        $results.Add(@{ name = 'Restore clipboard'; status = 'FAIL'; detail = "$_" })
    }
    try {
        if ($uiStarted) { Invoke-WinAppChecked (@('ui', 'yield') + $scopeArgs) | Out-Null }
    } catch {
        $results.Add(@{ name = 'Release UI workflow'; status = 'FAIL'; detail = "$_" })
    } finally {
        $env:WINAPP_UI_WORKFLOW_ID = $previousWorkflowId
        if ($ownsApp -and $Target -eq 'local') {
            $process = Get-Process -Id $AppPid -ErrorAction SilentlyContinue
            if ($process) { Stop-Process -Id $AppPid }
        }
    }
}

$failed = @($results | Where-Object { $_.status -eq 'FAIL' }).Count
$report = [ordered]@{
    target = $Target; appPid = $AppPid; workflowId = $WorkflowId
    passed = $results.Count - $failed; failed = $failed
    results = @($results.ToArray()); screenshots = @($screenshots.ToArray())
}
$reportPath = Join-Path $ArtifactDirectory 'test-results.json'
$report | ConvertTo-Json -Depth 6 | Set-Content -LiteralPath $reportPath -Encoding utf8
Write-Host "Passed: $($report.passed) | Failed: $failed | Results: $reportPath"
if ($failed) { exit 1 }
exit 0
