using WPFGallery.Services;
using WPFGallery.ViewModels;

namespace WPFGallery.Views;

public partial class ClipboardPage : Page
{
    public ClipboardPageViewModel ViewModel { get; }
    private readonly IClipboardService _clipboard;

    public ClipboardPage() : this(App.Services.GetRequiredService<ClipboardPageViewModel>(),
        App.Services.GetRequiredService<IClipboardService>())
    { }

    public ClipboardPage(ClipboardPageViewModel viewModel, IClipboardService clipboard)
    {
        ViewModel = viewModel;
        _clipboard = clipboard;
        InitializeComponent();
    }

    private void CopyToClipboard_Click(object sender, RoutedEventArgs args)
    {
        var text = CopyTextBox.Text;
        if (string.IsNullOrEmpty(text))
        {
            ViewModel.CopyStatus = "Nothing to copy - text box is empty.";
            return;
        }
        _clipboard.CopyText(text);
        ViewModel.CopyStatus = $"Copied \"{text}\" to clipboard!";
    }

    private async void PasteFromClipboard_Click(object sender, RoutedEventArgs args) =>
        ViewModel.PastedText = await _clipboard.ReadTextAsync() ?? "(No text in clipboard)";

    private void ClearClipboard_Click(object sender, RoutedEventArgs args)
    {
        _clipboard.Clear();
        ViewModel.ClearStatus = "Clipboard cleared!";
        ViewModel.PastedText = "";
    }

    private void CheckFormats_Click(object sender, RoutedEventArgs args) =>
        ViewModel.FormatsInfo = _clipboard.DescribeFormats();

    private void CopyImageToClipboard_Click(object sender, RoutedEventArgs args)
    {
        if (SourceImage.Source is BitmapImage { UriSource: { } source })
        {
            _clipboard.CopyImage(source);
            ViewModel.CopyImageStatus = "Image copied to clipboard!";
        }
        else
        {
            ViewModel.CopyImageStatus = "Failed to copy image.";
        }
    }

    private async void PasteImageFromClipboard_Click(object sender, RoutedEventArgs args)
    {
        var image = await _clipboard.ReadImageAsync();
        PastedImage.Source = image;
        PastedImage.Visibility = image is null ? Visibility.Collapsed : Visibility.Visible;
        ViewModel.PasteImageStatus = image is null
            ? "No image in clipboard." : $"Image pasted! Size: {image.PixelWidth}x{image.PixelHeight}";
    }
}
