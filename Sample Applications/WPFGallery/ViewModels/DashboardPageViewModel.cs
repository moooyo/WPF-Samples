using WPFGallery.Navigation;
using WPFGallery.Views;
using WPFGallery.Models;

namespace WPFGallery.ViewModels
{
    public partial class DashboardPageViewModel : ObservableObject
    {

        [ObservableProperty]
        public partial ICollection<ControlInfoDataItem> NavigationCards { get; set; } = ControlsInfoDataSource.Instance.GetGroupedControlsInfo();

        [ObservableProperty]
        public partial ICollection<ControlInfoDataItem> RecentlyAddedOrUpdatedSamplesInfo { get; set; } = ControlsInfoDataSource.Instance.GetRecentlyAddedOrUpdatedSamplesInfo();

        private readonly INavigationService _navigationService;

        public DashboardPageViewModel(INavigationService navigationService)
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
