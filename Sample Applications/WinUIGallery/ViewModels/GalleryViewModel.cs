using CommunityToolkit.Mvvm.ComponentModel;
using WPFGallery.Models;

namespace WPFGallery.ViewModels;

public partial class GalleryViewModel : ObservableObject
{
    [ObservableProperty]
    public partial string Title { get; set; } = "Home";

    [ObservableProperty]
    public partial string Description { get; set; } = "";

    [ObservableProperty]
    public partial IReadOnlyList<ControlInfoDataItem> Items { get; set; } = [];

    public void Show(ControlInfoDataItem item)
    {
        Title = item.Title;
        Description = item.Description;
        Items = item.PageName switch
        {
            "DashboardPage" => GalleryCatalog.Groups.Where(group => group.IsGroup).ToArray(),
            "AllSamplesPage" => GalleryCatalog.Groups
                .Where(group => group.UniqueId is not ("Samples" or "Design Guidance"))
                .SelectMany(group => group.Items).ToArray(),
            "WhatsNewPage" => GalleryCatalog.All.Where(sample => sample.IsNew || sample.IsUpdated).ToArray(),
            _ => item.Items
        };
    }
}
