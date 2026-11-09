
namespace WPFGallery.ViewModels
{
    public partial class LabelPageViewModel : ObservableObject
    {
        [ObservableProperty]
        public partial string PageTitle { get; set; } = "Label";

        [ObservableProperty]
        public partial string PageDescription { get; set; } = "";

    }
}
