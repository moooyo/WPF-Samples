using WPFGallery.Navigation;

namespace WPFGallery.ViewModels
{
    public partial class TypographyPageViewModel : ObservableObject
    {
        [ObservableProperty]
        public partial string PageTitle { get; set; } = "Typography";

        [ObservableProperty]
        public partial string PageDescription { get; set; } = "Guide showing how to use typography in your app";
    }
}
