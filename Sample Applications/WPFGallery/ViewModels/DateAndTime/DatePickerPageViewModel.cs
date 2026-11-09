
namespace WPFGallery.ViewModels;

public partial class DatePickerPageViewModel : ObservableObject
{
    [ObservableProperty]
    public partial string PageTitle { get; set; } = "DatePicker";

    [ObservableProperty]
    public partial string PageDescription { get; set; } = "";


}
