using WPFGallery.Navigation;
using WPFGallery.Views;

namespace WPFGallery.ViewModels
{
    public partial class SettingsPageViewModel : ObservableObject
    {
        [ObservableProperty]
        public partial string PageTitle { get; set; } = "Settings";

        public string PageDescription => string.Empty;

        [ObservableProperty]
        public partial int SelectedThemeIndex { get; set; } = 2;
    }
}
