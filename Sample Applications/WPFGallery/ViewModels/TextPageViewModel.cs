using WPFGallery.Navigation;
using WPFGallery.Views;
using WPFGallery.Models;

namespace WPFGallery.ViewModels
{
    public partial class TextPageViewModel : ObservableObject
    {
        [ObservableProperty]
        public partial string PageTitle { get; set; } = "Text";

        [ObservableProperty]
        public partial string PageDescription { get; set; } = "Controls for displaying and editing text";

        [ObservableProperty]
        public partial ICollection<ControlInfoDataItem> NavigationCards { get; set; } = ControlsInfoDataSource.Instance.GetControlsInfo("Text");
        private readonly INavigationService _navigationService;

        public TextPageViewModel(INavigationService navigationService)
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
