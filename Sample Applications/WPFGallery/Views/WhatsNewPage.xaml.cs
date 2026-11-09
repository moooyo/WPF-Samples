using WPFGallery.ViewModels;

namespace WPFGallery.Views;

public partial class WhatsNewPage : Page
{
    public WhatsNewPageViewModel ViewModel { get; }

    public WhatsNewPage() : this(App.Services.GetRequiredService<WhatsNewPageViewModel>()) { }

    public WhatsNewPage(WhatsNewPageViewModel viewModel)
    {
        ViewModel = viewModel;
        InitializeComponent();
    }

    private void NavigateToMessageBoxSample(object sender, RoutedEventArgs e) =>
        ViewModel.Navigate(typeof(MessageBoxPage));
}
