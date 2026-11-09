using WPFGallery.ViewModels;

namespace WPFGallery.Views
{
    /// <summary>
    /// Interaction logic for TextPage.xaml
    /// </summary>
    public partial class TextPage : Page
    {
        public TextPageViewModel ViewModel { get; }
        public TextPage() : this(App.Services.GetRequiredService<TextPageViewModel>()) { }

        public TextPage(TextPageViewModel viewModel)
        {
            InitializeComponent();
            ViewModel = viewModel;
            DataContext = this;
        }
    }
}
