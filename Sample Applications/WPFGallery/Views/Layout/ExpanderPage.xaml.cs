
using WPFGallery.ViewModels;


namespace WPFGallery.Views
{
    /// <summary>
    /// Interaction logic for ExpanderPage.xaml
    /// </summary>
    public partial class ExpanderPage : Page
    {
        public ExpanderPage() : this(App.Services.GetRequiredService<ExpanderPageViewModel>()) { }

        public ExpanderPage(ExpanderPageViewModel viewModel)
        {
            ViewModel = viewModel;
            DataContext = this;
            InitializeComponent();
        }

        public ExpanderPageViewModel ViewModel { get; }
    }
}
