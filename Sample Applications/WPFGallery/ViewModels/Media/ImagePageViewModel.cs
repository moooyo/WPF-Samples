
namespace WPFGallery.ViewModels;

public partial class ImagePageViewModel : ObservableObject
{
    [ObservableProperty]
    public partial string PageTitle { get; set; } = "Image";

    [ObservableProperty]
    public partial string PageDescription { get; set; } = "";


}
