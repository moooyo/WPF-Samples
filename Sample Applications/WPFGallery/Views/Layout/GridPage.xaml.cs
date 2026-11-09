using WPFGallery.ViewModels.Layout;

namespace WPFGallery.Views
{
    /// <summary>
    /// Interaction logic for GridPage.xaml
    /// </summary>
    public partial class GridPage : Page
    {
        public GridPage() : this(App.Services.GetRequiredService<GridPageViewModel>()) { }

        public GridPage(GridPageViewModel viewModel)
        {
            ViewModel = viewModel;
            DataContext = this;
            InitializeComponent();
        }

        public GridPageViewModel ViewModel { get; }

    }
}
