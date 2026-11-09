using WPFGallery.Helpers;
using WPFGallery.ViewModels;

namespace WPFGallery.Views
{
    /// <summary>
    /// Interaction logic for SpacingPage.xaml
    /// </summary>
    public partial class SpacingPage : Page
    {
        public SpacingPageViewModel ViewModel { get; }

        public SpacingPage() : this(App.Services.GetRequiredService<SpacingPageViewModel>()) { }

        public SpacingPage(SpacingPageViewModel viewModel)
        {
            InitializeComponent();

            ViewModel = viewModel;
            DataContext = this;
            Loaded += (_, _) => UpdateImageResources();
            ActualThemeChanged += (_, _) => UpdateImageResources();
        }

        private void UpdateImageResources()
        {
            if (ActualTheme == ElementTheme.Light)
            {
                DialogImage.Source = new BitmapImage(new Uri("ms-appx:///Assets/Design/Dialog.light.png"));
                CardImage.Source = new BitmapImage(new Uri("ms-appx:///Assets/Design/Cards.light.png"));
            }
            else
            {
                DialogImage.Source = new BitmapImage(new Uri("ms-appx:///Assets/Design/Dialog.dark.png"));
                CardImage.Source = new BitmapImage(new Uri("ms-appx:///Assets/Design/Cards.dark.png"));
            }
        }
    }
}
