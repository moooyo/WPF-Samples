using WPFGallery.Navigation;

namespace WPFGallery.ViewModels
{
    public partial class SpacingPageViewModel : ObservableObject
    {
        [ObservableProperty]
        public partial string PageTitle { get; set; } = "Spacing";

        [ObservableProperty]
        public partial string PageDescription { get; set; } = "Guide showing how to use spacing in your app";
    }
}
