
namespace WPFGallery.ViewModels
{
    public partial class TextBlockPageViewModel : ObservableObject
    {
        [ObservableProperty]
        public partial string PageTitle { get; set; } = "TextBlock";

        [ObservableProperty]
        public partial string PageDescription { get; set; } = "";

    }
}
