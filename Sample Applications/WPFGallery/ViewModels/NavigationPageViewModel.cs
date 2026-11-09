using WPFGallery.Navigation;
using WPFGallery.Views;
using WPFGallery.Models;

namespace WPFGallery.ViewModels
{
    public partial class NavigationPageViewModel : ObservableObject
    {
        [ObservableProperty]
        public partial string PageTitle { get; set; } = "Navigation";

        [ObservableProperty]
        public partial string PageDescription { get; set; } = "Controls for navigation and actions";

        [ObservableProperty]
        public partial ICollection<ControlInfoDataItem> NavigationCards { get; set; } = ControlsInfoDataSource.Instance.GetControlsInfo("Navigation");

        private readonly INavigationService _navigationService;

        public NavigationPageViewModel(INavigationService navigationService)
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
