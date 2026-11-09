namespace WPFGallery.Helpers;

internal static class Utility
{
    public static bool IsBackdropDisabled() =>
        AppContext.TryGetSwitch("Switch.System.Windows.Appearance.DisableFluentThemeWindowBackdrop", out var disabled) && disabled;

    public static bool IsLightTheme() =>
        App.MainWindow.Content is FrameworkElement { ActualTheme: ElementTheme.Light };
}
