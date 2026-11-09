# Gallery (WinUI 3)

A packaged WinUI 3 migration of the adjacent WPF Gallery. The original WPF
project remains unchanged for comparison. This app uses the `WPFGallery`
namespace, but does not reference its WPF assemblies or project.

## Build and run

Requires Windows, the .NET 10 SDK, WinApp CLI 0.7 or later, and Developer Mode
for development registration. The project targets x64 and ARM64, not AnyCPU.

From this directory:

```powershell
dotnet build .\WinUIGallery.csproj -p:Platform=x64
winapp run .\WinUIGallery.csproj --arch x64 --detach --json
```

Open `WinUIGallery.slnx` in Visual Studio to develop this app independently.
Launch through `winapp run`, rather than the executable, to preserve package
identity. `Package.appxmanifest` remains part of the project.

The app uses JIT compilation. Native AOT is not configured. Sample markup is
loaded with `XamlReader` so the displayed XAML and rendered basic examples stay
in sync; publishing is deliberately untrimmed. Data-driven views use compiled,
typed `x:Bind` templates and CommunityToolkit.Mvvm source generators.

## Migration map

The original catalog routes, category pages, recently updated samples, design
guidance, icon search/pagination, user dashboard, and system samples are retained.
The Canvas and Image pages are also exposed under Media.

| WPF | WinUI implementation |
| --- | --- |
| TreeView app navigation and navigation service | NavigationView and Frame, synchronized selection and back stack |
| DataGrid / ListView.GridView | ListView with matching Grid headers and typed rows |
| Calendar / DatePicker | CalendarView / CalendarDatePicker and DatePicker |
| TabControl | TabView with adding and closing |
| Menu / ToolBar | MenuBar / CommandBar |
| GroupBox / Label | Labelled containers / control headers and LabeledBy |
| GridSplitter | CommunityToolkit.WinUI.Controls.Sizers |
| ResizeGrip / NavigationWindow | Native window resizing / Window hosting Frame |
| RichTextBox | RichEditBox with formatting commands |
| MessageBox | XamlRoot-owned ContentDialog |
| Microsoft.Win32 dialogs | WindowId-owned Microsoft.Windows.Storage.Pickers |
| WPF Clipboard / BitmapSource | DataPackage / BitmapImage and random-access streams |
| DynamicResource / pack URI | ThemeResource / ms-appx URI |
| Custom property notification | CommunityToolkit.Mvvm observable partial properties |

WPF-only Grid shorthand, AccentColor APIs, and text rendering internals are not
carried into WinUI; corresponding pages demonstrate native WinUI APIs instead.
This is a framework migration, not a pixel-identical reproduction of WPF styles.
The app uses native theme brushes, focus visuals, and accessible control headers.
The selected theme is persisted in packaged local settings.

User dashboard edits are in-memory demo data, like the original sample, not a
database. Save validates required names and age; Cancel discards the draft.
Deleting a user requires confirmation. Clipboard and file samples operate only
when their buttons are explicitly used.

## Validation

```powershell
dotnet format .\WinUIGallery.csproj --verify-no-changes --no-restore
dotnet build .\WinUIGallery.csproj -p:Platform=x64
.\Tests\ui-tests.ps1 -Target sandbox
```

Use `-Target local` only when Sandbox is unavailable and desktop interaction is
acceptable. A live app may be reused with `-AppPid <PID> -AppScope local`
(or matching Sandbox scope). The script traverses every catalog route and checks
buttons, tri-state selection, validation, frame history, icon filtering,
ContentDialogs, dashboard transactions, live themes, and cancellation of all
four file/folder picker scenarios. It writes a JSON report
and screenshots to a unique `Tests\artifacts` directory, or `-ArtifactDirectory`.
Review the screenshots in addition to the automated assertions.

The suite deliberately does not overwrite the shared clipboard or modify the
machine's Windows Contrast settings. Validate clipboard text/image round trips,
file-picker selection and saved file contents, keyboard focus, and Windows Contrast themes on an
isolated interactive desktop before releasing.
