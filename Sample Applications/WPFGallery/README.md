# WPF Gallery — WinUI 3 migration

The Gallery now runs on WinUI 3 and .NET 10. The `WPFGallery` assembly name,
existing page names, navigation catalog, images, and sample data are retained.
The other WPF samples in this repository are unchanged.

## Build and run

Use Windows 10 build 19041 or later, the .NET SDK selected by the repository's
`global.json`, and WinApp CLI 0.7 or later. Visual Studio must include the WinUI
development tools when building or deploying from the IDE.

From this directory:

```powershell
dotnet restore .\WPFGallery.csproj -p:Platform=x64
dotnet build .\WPFGallery.sln -p:Platform=x64 --no-restore
winapp run .\WPFGallery.csproj --no-build
```

Use `x64`, `x86`, or `ARM64` for `Platform`. The project selects the corresponding
runtime identifier so solution builds and CLI activation use the same output.
Both solution files use single-project MSIX tooling; the former, missing
DesktopWap packaging project is no longer referenced.

If nuget.org is unavailable, the public Microsoft mirror can be selected for
this restore without changing machine-wide settings:

```powershell
dotnet restore .\WPFGallery.csproj -p:Platform=x64 --source https://pkgs.dev.azure.com/dnceng/public/_packaging/dotnet-public/nuget/v3/index.json
```

## Control equivalents

| Original sample | WinUI implementation |
| --- | --- |
| DataGrid / GridView columns | Editable, sortable ListView with typed Grid rows |
| Calendar / DatePicker | CalendarView / CalendarDatePicker |
| TabControl / Menu | TabView / MenuBar and MenuFlyout |
| GroupBox / Label | Border with a heading / text or content controls |
| RichTextBox | RichEditBox |
| GridSplitter | Community Toolkit GridSplitter |
| NavigationWindow / Frame | Tracked WindowEx with Frame navigation |
| ResizeGrip | Resizable WindowEx sample window |
| MessageBox | Owned ContentDialog with equivalent button results |
| File and folder dialogs | Asynchronous desktop pickers bound to WindowId |
| Clipboard | Windows DataTransfer text and image services |

Editable text uses explicit two-way bindings. Validation displays an accessible
error without overwriting the last valid value. Dashboard deletion requires
confirmation and defaults to Cancel. Settings applies Light, Dark, or the system
theme to the hosted root rather than using WPF ThemeMode.

To disable Mica while preserving the original configuration switch:

```xml
<ItemGroup>
  <RuntimeHostConfigurationOption Include="Switch.System.Windows.Appearance.DisableFluentThemeWindowBackdrop" Value="true" />
</ItemGroup>
```

## UI verification

Build Debug first, then run the host-side batch on the intended test desktop:

```powershell
pwsh -NoProfile -STA -File .\ui-tests.ps1 -Target local
```

UI tests require PowerShell 7.3 or later. Use `-Target sandbox` when Windows Sandbox is available. The script does not
enable Windows features or fall back to another target. Local tests preserve
the clipboard in memory, scope input to the Gallery, and stop only the app
instance they launch. It verifies the catalog pages and key interactions,
checks native command exit codes, and writes screenshots and
`test-results.json` under `obj\ui-test-artifacts`. Review the screenshots as well
as the assertions. Contrast themes require a separately configured test
desktop; the batch does not change system accessibility settings.

## Deployment

The package identity is `Microsoft.WPFGallery.WinUI`, with publisher
`CN=Microsoft`, as declared in `Package.appxmanifest`. Development activation is
packaged; deployment remains framework-dependent on .NET and the Windows App SDK.

```powershell
winapp package .\WPFGallery.csproj --arch x64 --no-sign
```

This builds Release and reports the unsigned MSIX artifact path. Use the
organization's approved certificate and signing pipeline before distribution;
never commit certificates or passwords. An unsigned package is not an
installable release. The build pipeline separates the Gallery's native
architectures from the remaining WPF samples' Any CPU matrix.
