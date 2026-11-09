using WPFGallery.Navigation;

namespace WPFGallery.ViewModels
{
    public partial class ColorsPageViewModel : ObservableObject
    {
        [ObservableProperty]
        public partial string PageTitle { get; set; } = "Colors";

        [ObservableProperty]
        public partial string PageDescription { get; set; } = "Guide showing how to use colors in your app";

    }
}
