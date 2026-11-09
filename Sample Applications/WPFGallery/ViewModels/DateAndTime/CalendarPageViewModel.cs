
namespace WPFGallery.ViewModels;

public partial class CalendarPageViewModel : ObservableObject
{
    [ObservableProperty]
    public partial string PageTitle { get; set; } = "Calendar";

    [ObservableProperty]
    public partial string PageDescription { get; set; } = "";


}
