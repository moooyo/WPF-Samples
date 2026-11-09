using WPFGallery.ViewModels;

namespace WPFGallery.Views
{
    /// <summary>
    /// Interaction logic for TypographyPage.xaml
    /// </summary>
    public partial class TypographyPage : Page
    {
        public TypographyPage() : this(App.Services.GetRequiredService<TypographyPageViewModel>()) { }

        public TypographyPage(TypographyPageViewModel viewModel)
        {
            InitializeComponent();
            ViewModel = viewModel;
            DataContext = this;
        }

        public TypographyPageViewModel ViewModel { get; }
    }
}
