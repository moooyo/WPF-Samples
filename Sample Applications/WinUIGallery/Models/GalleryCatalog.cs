using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace WPFGallery.Models;

public sealed class ControlInfoDataItem
{
    public string UniqueId { get; set; } = "";
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public string IconGlyph { get; set; } = "";
    public string ImagePath { get; set; } = "";
    public string PageName { get; set; } = "";
    public bool IsGroup { get; set; }
    public bool IsNew { get; set; }
    public bool IsUpdated { get; set; }
    public List<ControlInfoDataItem> Items { get; set; } = [];

    [JsonIgnore]
    public string ImageSource => string.IsNullOrEmpty(ImagePath)
        ? Items.FirstOrDefault(item => item.ImagePath.Length > 0)?.ImageSource
            ?? "ms-appx:///Assets/Gallery/AppIcons/WPFGallery_48px.png"
        : $"ms-appx:///Assets/Gallery/{ImagePath["Assets/".Length..]}";

    public override string ToString() => Title;
}

public sealed class IconData
{
    public string Name { get; set; } = "";
    public string Code { get; set; } = "";
    public List<string> Tags { get; set; } = [];
    [JsonIgnore]
    public string Character => char.ConvertFromUtf32(Convert.ToInt32(Code, 16));
    [JsonIgnore]
    public string TextGlyph => $"&#x{Code};";
}

[JsonSerializable(typeof(List<ControlInfoDataItem>))]
[JsonSerializable(typeof(List<IconData>))]
internal partial class GalleryJsonContext : JsonSerializerContext;

public static class GalleryCatalog
{
    public static IReadOnlyList<ControlInfoDataItem> Groups { get; } = ReadControls();
    public static IReadOnlyList<ControlInfoDataItem> All { get; } =
        Groups.SelectMany(group => new[] { group }.Concat(group.Items)).ToArray();
    public static IReadOnlyList<IconData> Icons { get; } = ReadIcons();
    public static ControlInfoDataItem Settings { get; } = new()
    {
        UniqueId = "Settings",
        Title = "Settings",
        PageName = "SettingsPage",
        Description = "Appearance, app information, and useful links."
    };

    public static ControlInfoDataItem Find(string pageName) =>
        pageName == Settings.PageName ? Settings :
        All.Single(item => item.PageName == pageName);

    private static List<ControlInfoDataItem> ReadControls()
    {
        using Stream stream = Open("ControlsInfoData");
        return JsonSerializer.Deserialize(stream, GalleryJsonContext.Default.ListControlInfoDataItem)
            ?? throw new InvalidDataException("The Gallery catalog is empty.");
    }

    private static List<IconData> ReadIcons()
    {
        using Stream stream = Open("IconsData");
        return JsonSerializer.Deserialize(stream, GalleryJsonContext.Default.ListIconData)
            ?? throw new InvalidDataException("The icon catalog is empty.");
    }

    private static Stream Open(string name) =>
        Assembly.GetExecutingAssembly().GetManifestResourceStream($"WPFGallery.Models.{name}.json")
        ?? throw new InvalidDataException($"Missing Gallery resource: {name}.");
}
