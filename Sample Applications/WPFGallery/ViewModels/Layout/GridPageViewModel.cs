namespace WPFGallery.ViewModels.Layout
{
    public partial class GridPageViewModel : ObservableObject
    {
        [ObservableProperty]
        public partial string PageTitle { get; set; } = "Grid";

        [ObservableProperty]
        public partial string PageDescription { get; set; } = "";

        public GridPageViewModel() { }
    }
}
