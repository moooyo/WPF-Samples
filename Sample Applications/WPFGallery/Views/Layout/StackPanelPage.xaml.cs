
using WPFGallery.ViewModels;
using WPFGallery.ViewModels.Layout;

namespace WPFGallery.Views
{
    /// <summary>
    /// Interaction logic for StackPanelPage.xaml
    /// </summary>
    public partial class StackPanelPage : Page
    {
        public StackPanelPage() : this(App.Services.GetRequiredService<StackPanelPageViewModel>()) { }

        public StackPanelPage(StackPanelPageViewModel viewModel)
        {
            ViewModel = viewModel;
            DataContext = this;
            InitializeComponent();
        }

        public StackPanelPageViewModel ViewModel { get; }
    }
}
