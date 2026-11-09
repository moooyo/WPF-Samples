
using WPFGallery.ViewModels;

namespace WPFGallery.Views
{
    /// <summary>
    /// Interaction logic for ImagePage.xaml
    /// </summary>
    public partial class ImagePage : Page
    {
        public ImagePageViewModel ViewModel { get; }

        public ImagePage() : this(App.Services.GetRequiredService<ImagePageViewModel>()) { }

        public ImagePage(ImagePageViewModel viewModel)
        {
            ViewModel = viewModel;
            DataContext = this;

            InitializeComponent();
        }
    }
}
