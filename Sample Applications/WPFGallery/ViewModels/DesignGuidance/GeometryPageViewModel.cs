using WPFGallery.Navigation;

namespace WPFGallery.ViewModels
{
    public partial class GeometryPageViewModel : ObservableObject
    {
        [ObservableProperty]
        public partial string PageTitle { get; set; } = "Geometry";

        [ObservableProperty]
        public partial string PageDescription { get; set; } = "";
    }
}
