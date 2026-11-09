using WPFGallery.ViewModels;

namespace WPFGallery.Views
{
    /// <summary>
    /// Interaction logic for SamplesPage.xaml
    /// </summary>
    public partial class SamplesPage : Page
    {
        public SamplesPageViewModel ViewModel { get; }
        public SamplesPage() : this(App.Services.GetRequiredService<SamplesPageViewModel>()) { }

        public SamplesPage(SamplesPageViewModel viewModel)
        {
            InitializeComponent();
            ViewModel = viewModel;
            DataContext = this;
        }
    }
}
