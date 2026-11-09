
namespace WPFGallery.ViewModels;

public partial class ClipboardPageViewModel : ObservableObject
{
    [ObservableProperty]
    public partial string PageTitle { get; set; } = "Clipboard";

    [ObservableProperty]
    public partial string PageDescription { get; set; } = "";

    [ObservableProperty]
    public partial string CopyStatus { get; set; } = "";

    [ObservableProperty]
    public partial string PastedText { get; set; } = "";

    [ObservableProperty]
    public partial string ClearStatus { get; set; } = "";

    [ObservableProperty]
    public partial string FormatsInfo { get; set; } = "";

    [ObservableProperty]
    public partial string CopyImageStatus { get; set; } = "";

    [ObservableProperty]
    public partial string PasteImageStatus { get; set; } = "";

    public ClipboardPageViewModel() { }
}
