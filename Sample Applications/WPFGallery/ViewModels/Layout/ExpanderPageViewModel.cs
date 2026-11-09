
namespace WPFGallery.ViewModels;

public partial class ExpanderPageViewModel : ObservableObject
{
    [ObservableProperty]
    public partial string PageTitle { get; set; } = "Expander";

    [ObservableProperty]
    public partial string PageDescription { get; set; } = "";

    public ExpanderPageViewModel() { }
}
