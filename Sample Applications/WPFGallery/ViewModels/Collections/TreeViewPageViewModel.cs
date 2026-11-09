
namespace WPFGallery.ViewModels;

public partial class TreeViewPageViewModel : ObservableObject
{
    [ObservableProperty]
    public partial string PageTitle { get; set; } = "TreeView";

    [ObservableProperty]
    public partial string PageDescription { get; set; } = "";


}
