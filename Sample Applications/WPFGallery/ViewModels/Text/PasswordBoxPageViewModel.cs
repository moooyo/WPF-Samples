
namespace WPFGallery.ViewModels
{
    public partial class PasswordBoxPageViewModel : ObservableObject
    {
        [ObservableProperty]
        public partial string PageTitle { get; set; } = "PasswordBox";

        [ObservableProperty]
        public partial string PageDescription { get; set; } = "";

    }
}
