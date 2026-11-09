
namespace WPFGallery.ViewModels.Layout;

public partial class StackPanelPageViewModel : ObservableObject
{
    [ObservableProperty]
    public partial string PageTitle { get; set; } = "StackPanel";

    [ObservableProperty]
    public partial string PageDescription { get; set; } = "";

    public StackPanelPageViewModel() { }
}
