using WPFGallery.Navigation;

namespace WPFGallery.ViewModels;

public partial class WhatsNewPageViewModel(INavigationService navigationService) : ObservableObject
{
    public string PageTitle => "What's new in this Gallery";
    public string PageDescription => "The WPF Gallery migrated to WinUI 3, with native controls, Fluent resources, and asynchronous desktop services.";

    [RelayCommand]
    public void Navigate(Type pageType) => navigationService.Navigate(pageType);
}
