
using WPFGallery.ViewModels;
namespace WPFGallery.Views
{
    /// <summary>
    /// Interaction logic for SystemPage.xaml
    /// </summary>
    public partial class SystemPage : Page
    {
        public SystemPageViewModel ViewModel { get; }
        public SystemPage() : this(App.Services.GetRequiredService<SystemPageViewModel>()) { }

        public SystemPage(SystemPageViewModel viewModel)
        {
            InitializeComponent();
            ViewModel = viewModel;
            DataContext = this;
        }
    }
}
