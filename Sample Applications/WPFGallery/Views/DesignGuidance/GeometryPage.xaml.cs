using WPFGallery.Helpers;
using WPFGallery.ViewModels;

namespace WPFGallery.Views
{
    /// <summary>
    /// Interaction logic for GeometryPage.xaml
    /// </summary>
    public partial class GeometryPage : Page
    {
        public GeometryPageViewModel ViewModel { get; }

        public GeometryPage() : this(App.Services.GetRequiredService<GeometryPageViewModel>()) { }

        public GeometryPage(GeometryPageViewModel viewModel)
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
                GeometryImage.Source = new BitmapImage(new Uri("ms-appx:///Assets/Design/Geometry.light.png"));
            }
            else
            {
                GeometryImage.Source = new BitmapImage(new Uri("ms-appx:///Assets/Design/Geometry.dark.png"));
            }
        }
    }
}
