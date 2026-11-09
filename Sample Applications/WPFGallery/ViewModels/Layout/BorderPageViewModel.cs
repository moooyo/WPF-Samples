
namespace WPFGallery.ViewModels.Layout;

public partial class BorderPageViewModel : ObservableObject
{
    [ObservableProperty]
    public partial string PageTitle { get; set; } = "Border";

    [ObservableProperty]
    public partial string PageDescription { get; set; } = "";

    public BorderPageViewModel() { }
}
