using WPFGallery.ViewModels;

namespace WPFGallery.Views;

public partial class DataGridPage : Page
{
    public DataGridPageViewModel ViewModel { get; }

    public DataGridPage() : this(App.Services.GetRequiredService<DataGridPageViewModel>()) { }

    public DataGridPage(DataGridPageViewModel viewModel)
    {
        ViewModel = viewModel;
        InitializeComponent();
    }

    private void Sort_Click(object sender, RoutedEventArgs args) =>
        ViewModel.SortCommand.Execute((string)((Button)sender).Tag);
}
