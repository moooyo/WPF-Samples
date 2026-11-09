using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using WPFGallery.Models;
using WPFGallery.Samples;
using WPFGallery.ViewModels;

namespace WPFGallery.Views;

public sealed partial class GalleryPage : Page
{
    public GalleryViewModel ViewModel { get; } = new();
    public string Route { get; private set; } = "";

    public GalleryPage() => InitializeComponent();

    protected override void OnNavigatedTo(NavigationEventArgs args)
    {
        base.OnNavigatedTo(args);
        Route = (string)args.Parameter;
        ControlInfoDataItem item = GalleryCatalog.Find(Route);
        ViewModel.Show(item);
        bool isCatalog = item.IsGroup || Route is "DashboardPage" or "AllSamplesPage" or "WhatsNewPage";
        CatalogGrid.Visibility = isCatalog ? Visibility.Visible : Visibility.Collapsed;
        SampleScroll.Visibility = isCatalog ? Visibility.Collapsed : Visibility.Visible;
        if (isCatalog)
        {
            if (Route == "DashboardPage")
            {
                ViewModel.Description = "Explore the WPF Gallery catalog, rebuilt with native WinUI 3 controls.";
            }
            else if (Route == "WhatsNewPage")
            {
                ViewModel.Description = "Recently added and updated samples. WPF-specific APIs are replaced by native WinUI equivalents.";
            }
        }
        else
        {
            SampleFactory.Populate(Route, SampleContent);
        }
    }

    private void CatalogGrid_ItemClick(object sender, ItemClickEventArgs args) =>
        MainPage.Current.Navigate((ControlInfoDataItem)args.ClickedItem);
}
