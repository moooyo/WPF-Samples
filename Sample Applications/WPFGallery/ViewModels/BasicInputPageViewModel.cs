using WPFGallery.Navigation;
using WPFGallery.Views;
using WPFGallery.Models;

namespace WPFGallery.ViewModels
{
    public partial class BasicInputPageViewModel : ObservableObject
    {
        [ObservableProperty]
        public partial string PageTitle { get; set; } = "Basic Input";

        [ObservableProperty]
        public partial string PageDescription { get; set; } = "Controls for getting user input";

        [ObservableProperty]
        public partial ICollection<ControlInfoDataItem> NavigationCards { get; set; } = ControlsInfoDataSource.Instance.GetControlsInfo("Basic Input");

        private readonly INavigationService _navigationService;

        public BasicInputPageViewModel(INavigationService navigationService)
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
