
using WPFGallery.ViewModels;

namespace WPFGallery.Views
{
    /// <summary>
    /// Interaction logic for TextBoxPage.xaml
    /// </summary>
    public partial class TextBoxPage : Page
    {
        public TextBoxPageViewModel ViewModel { get; }
        public TextBoxPage() : this(App.Services.GetRequiredService<TextBoxPageViewModel>()) { }

        public TextBoxPage(TextBoxPageViewModel viewModel)
        {
            ViewModel = viewModel;
            DataContext = this;
            InitializeComponent();
        }
    }
}
