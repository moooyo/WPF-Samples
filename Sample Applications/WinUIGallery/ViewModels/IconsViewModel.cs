using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WPFGallery.Models;

namespace WPFGallery.ViewModels;

public partial class IconsViewModel : ObservableObject
{
    [ObservableProperty]
    public partial string SearchText { get; set; } = "";
    [ObservableProperty]
    public partial IReadOnlyList<IconData> DisplayedIcons { get; set; } = [];
    [ObservableProperty, NotifyPropertyChangedFor(nameof(SelectedName)),
     NotifyPropertyChangedFor(nameof(SelectedGlyph)), NotifyPropertyChangedFor(nameof(SelectedTags))]
    public partial IconData? SelectedIcon { get; set; }
    [ObservableProperty]
    public partial int SelectedPageSizeIndex { get; set; } = 1;
    [ObservableProperty]
    public partial string PageSummary { get; set; } = "";

    private IReadOnlyList<IconData> _filtered = GalleryCatalog.Icons;
    private int _page = 1;
    private int PageSize => new[] { 100, 250, 500, 1000, int.MaxValue }[SelectedPageSizeIndex];
    private int TotalPages => PageSize == int.MaxValue ? 1 : Math.Max(1, (int)Math.Ceiling((double)_filtered.Count / PageSize));
    public IReadOnlyList<string> PageSizes { get; } = ["100", "250", "500", "1000", "All"];
    public bool CanGoBack => _page > 1;
    public bool CanGoNext => _page < TotalPages;
    public string SelectedName => SelectedIcon?.Name ?? "No icons match your search.";
    public string SelectedGlyph => SelectedIcon?.TextGlyph ?? "";
    public IReadOnlyList<string> SelectedTags => SelectedIcon?.Tags ?? [];

    public IconsViewModel() => Refresh();
    partial void OnSearchTextChanged(string value)
    {
        _filtered = GalleryCatalog.Icons.Where(icon =>
            icon.Name.Contains(value, StringComparison.OrdinalIgnoreCase)
            || icon.Code.Contains(value, StringComparison.OrdinalIgnoreCase)
            || icon.Tags.Any(tag => tag.Contains(value, StringComparison.OrdinalIgnoreCase))).ToArray();
        _page = 1;
        Refresh();
    }
    partial void OnSelectedPageSizeIndexChanged(int value)
    {
        _page = 1;
        Refresh();
    }

    [RelayCommand(CanExecute = nameof(CanGoBack))]
    private void PreviousPage()
    {
        _page--;
        Refresh();
    }
    [RelayCommand(CanExecute = nameof(CanGoNext))]
    private void NextPage()
    {
        _page++;
        Refresh();
    }

    private void Refresh()
    {
        DisplayedIcons = PageSize == int.MaxValue ? _filtered : _filtered.Skip((_page - 1) * PageSize).Take(PageSize).ToArray();
        SelectedIcon = DisplayedIcons.FirstOrDefault();
        PageSummary = $"{_filtered.Count} icons - Page {_page} of {TotalPages}";
        PreviousPageCommand.NotifyCanExecuteChanged();
        NextPageCommand.NotifyCanExecuteChanged();
    }
}
