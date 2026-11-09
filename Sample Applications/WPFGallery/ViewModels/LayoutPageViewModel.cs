using WPFGallery.Navigation;
using WPFGallery.Views;
using WPFGallery.Models;

namespace WPFGallery.ViewModels
{
    public partial class LayoutPageViewModel : ObservableObject
    {
        [ObservableProperty]
        public partial string PageTitle { get; set; } = "Layout";

        [ObservableProperty]
        public partial string PageDescription { get; set; } = "Controls for layouting";

        [ObservableProperty]
        public partial ICollection<ControlInfoDataItem> NavigationCards { get; set; } = ControlsInfoDataSource.Instance.GetControlsInfo("Layout");

        private readonly INavigationService _navigationService;

        public LayoutPageViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
        }

        [RelayCommand]
        public void Navigate(object pageType)
        {
            if (pageType is Type page)
            {
                _navigationService.NavigateTo(page);
            }
        }


    }
}
