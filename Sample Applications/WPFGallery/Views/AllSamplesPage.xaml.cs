using WPFGallery.ViewModels;

namespace WPFGallery.Views
{
    /// <summary>
    /// Interaction logic for AllSamplesPage.xaml
    /// </summary>
    public partial class AllSamplesPage : Page
    {
        public AllSamplesPageViewModel ViewModel { get; }
        public AllSamplesPage() : this(App.Services.GetRequiredService<AllSamplesPageViewModel>()) { }

        public AllSamplesPage(AllSamplesPageViewModel viewModel)
        {
            InitializeComponent();
            ViewModel = viewModel;
            DataContext = this;
        }
    }
}
