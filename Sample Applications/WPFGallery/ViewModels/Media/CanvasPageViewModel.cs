
namespace WPFGallery.ViewModels;

public partial class CanvasPageViewModel : ObservableObject
{
    [ObservableProperty]
    public partial string PageTitle { get; set; } = "Canvas";

    [ObservableProperty]
    public partial string PageDescription { get; set; } = "";


}
