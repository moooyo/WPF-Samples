using WPFGallery.ViewModels;

namespace WPFGallery.Views;

public partial class IconsPage : Page
{
    public IconsPageViewModel ViewModel { get; }

    public IconsPage() : this(App.Services.GetRequiredService<IconsPageViewModel>()) { }

    public IconsPage(IconsPageViewModel viewModel)
    {
        ViewModel = viewModel;
        InitializeComponent();
        Loaded += async (_, _) =>
        {
            if (ViewModel.AllIcons.Count == 0)
                await ViewModel.LoadDataCommand.ExecuteAsync(null);
        };
    }

    private void Tag_Click(object sender, RoutedEventArgs args) =>
        ViewModel.ApplyTagFilterCommand.Execute(((Button)sender).Content as string);

    private void Open_SegoeFontDownloadPage(object sender, RoutedEventArgs args) =>
        Process.Start(new ProcessStartInfo("https://learn.microsoft.com/windows/apps/design/downloads/#fonts") { UseShellExecute = true });

    private void Open_IconDesignGuidelinesPage(object sender, RoutedEventArgs args) =>
        Process.Start(new ProcessStartInfo("https://learn.microsoft.com/windows/apps/design/style/segoe-fluent-icons-font#layering-and-mirroring") { UseShellExecute = true });
}
