using Microsoft.UI.Xaml;
using Microsoft.UI.Windowing;
using System.Runtime.InteropServices;
using Windows.Graphics;
using Windows.Storage;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WPFGallery;

/// <summary>
/// The application window. This hosts a Frame that displays pages. Add your
/// UI and logic to MainPage.xaml / MainPage.xaml.cs instead of here so you
/// can use Page features such as navigation events and the Loaded lifecycle.
/// </summary>
public sealed partial class MainWindow : Window
{
    [DllImport("user32.dll")]
    private static extern uint GetDpiForWindow(nint hwnd);

    public MainWindow()
    {
        InitializeComponent();

        ExtendsContentIntoTitleBar = true;
        SetTitleBar(AppTitleBar);

        AppWindow.SetIcon(Path.Combine(AppContext.BaseDirectory, "Assets", "AppIcon.ico"));
        double scale = GetDpiForWindow(WinRT.Interop.WindowNative.GetWindowHandle(this)) / 96.0;
        AppWindow.Resize(new SizeInt32((int)(1200 * scale), (int)(800 * scale)));
        if (AppWindow.Presenter is OverlappedPresenter presenter)
        {
            presenter.PreferredMinimumWidth = (int)(780 * scale);
            presenter.PreferredMinimumHeight = (int)(470 * scale);
        }
        AppTitleBar.ActualThemeChanged += (_, _) => UpdateTitleBarTheme();
        if (ApplicationData.Current.LocalSettings.Values["Theme"] is string savedTheme
            && Enum.TryParse(savedTheme, out ElementTheme theme))
        {
            ((FrameworkElement)Content).RequestedTheme = theme;
        }
        UpdateTitleBarTheme();

        // Navigate the root frame to the main page on startup.
        RootFrame.Navigate(typeof(MainPage));
    }

    private void UpdateTitleBarTheme() =>
        AppWindow.TitleBar.PreferredTheme = AppTitleBar.ActualTheme == ElementTheme.Dark
            ? TitleBarTheme.Dark : TitleBarTheme.Light;
}
