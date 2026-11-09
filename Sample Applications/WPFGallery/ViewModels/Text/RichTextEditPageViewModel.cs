
namespace WPFGallery.ViewModels
{
    public partial class RichTextEditPageViewModel : ObservableObject
    {
        [ObservableProperty]
        public partial string PageTitle { get; set; } = "RichTextEdit";

        [ObservableProperty]
        public partial string PageDescription { get; set; } = "";

    }
}
