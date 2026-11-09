using WPFGallery.ViewModels;

namespace WPFGallery.Views;

public partial class ComboBoxPage : Page
{
    public ComboBoxPageViewModel ViewModel { get; }

    public ComboBoxPage() : this(App.Services.GetRequiredService<ComboBoxPageViewModel>()) { }

    public ComboBoxPage(ComboBoxPageViewModel viewModel)
    {
        ViewModel = viewModel;
        InitializeComponent();
    }
}
