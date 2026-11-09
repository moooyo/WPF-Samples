using WPFGallery.Models;
using WPFGallery.Navigation;

namespace WPFGallery.Resources;

public partial class Templates : ResourceDictionary
{
    public Templates() => InitializeComponent();

    private void NavigationCard_Click(object sender, RoutedEventArgs args)
    {
        if (((Button)sender).DataContext is ControlInfoDataItem item)
            App.Services.GetRequiredService<INavigationService>().Navigate(item.PageType);
    }
}
