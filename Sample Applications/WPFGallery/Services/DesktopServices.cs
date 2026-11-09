using System.IO;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.Storage.Streams;
using Microsoft.Windows.Storage.Pickers;

namespace WPFGallery.Services;

public enum DialogButtons { OK, OKCancel, AbortRetryIgnore, YesNoCancel, YesNo, RetryCancel, CancelTryContinue }
public enum DialogImage { None, Error, Question, Warning, Information }
public enum DialogResult { OK, Cancel, Abort, Retry, Ignore, Yes, No, TryAgain, Continue }

public interface IDialogService
{
    Task<DialogResult> ShowAsync(XamlRoot root, string message, string title = "WPF Gallery",
        DialogButtons buttons = DialogButtons.OK, DialogImage image = DialogImage.None,
        DialogResult? defaultResult = null, string? primaryButtonText = null);
}

public sealed class DialogService : IDialogService
{
    public async Task<DialogResult> ShowAsync(XamlRoot root, string message, string title = "WPF Gallery",
        DialogButtons buttons = DialogButtons.OK, DialogImage image = DialogImage.None,
        DialogResult? defaultResult = null, string? primaryButtonText = null)
    {
        var (primary, secondary, close) = buttons switch
        {
            DialogButtons.OK => (DialogResult.OK, (DialogResult?)null, (DialogResult?)null),
            DialogButtons.OKCancel => (DialogResult.OK, null, DialogResult.Cancel),
            DialogButtons.AbortRetryIgnore => (DialogResult.Abort, DialogResult.Retry, DialogResult.Ignore),
            DialogButtons.YesNoCancel => (DialogResult.Yes, DialogResult.No, DialogResult.Cancel),
            DialogButtons.YesNo => (DialogResult.Yes, DialogResult.No, null),
            DialogButtons.RetryCancel => (DialogResult.Retry, null, DialogResult.Cancel),
            DialogButtons.CancelTryContinue => (DialogResult.TryAgain, DialogResult.Continue, DialogResult.Cancel),
            _ => throw new ArgumentOutOfRangeException(nameof(buttons))
        };
        var defaultButtonResult = defaultResult ??
            (buttons == DialogButtons.CancelTryContinue ? DialogResult.Cancel : primary);
        if (defaultButtonResult != primary && defaultButtonResult != secondary && defaultButtonResult != close)
        {
            throw new ArgumentOutOfRangeException(nameof(defaultResult), "The default result must match a dialog button.");
        }
        var content = new StackPanel { Spacing = 12 };
        if (image != DialogImage.None)
        {
            content.Children.Add(new FontIcon
            {
                Glyph = image switch
                {
                    DialogImage.Error => "\uEA39",
                    DialogImage.Question => "\uE9CE",
                    DialogImage.Warning => "\uE7BA",
                    DialogImage.Information => "\uE946",
                    _ => throw new ArgumentOutOfRangeException(nameof(image))
                }
            });
        }
        content.Children.Add(new TextBlock { Text = message, TextWrapping = TextWrapping.Wrap });
        var dialog = new ContentDialog
        {
            XamlRoot = root,
            RequestedTheme = App.MainWindow.RootTheme,
            Title = title,
            Content = content,
            PrimaryButtonText = primaryButtonText ?? Label(primary),
            SecondaryButtonText = secondary is { } s ? Label(s) : string.Empty,
            CloseButtonText = close is { } c ? Label(c) : string.Empty,
            DefaultButton = defaultButtonResult == secondary && secondary is not null
                ? ContentDialogButton.Secondary
                : defaultButtonResult == close && close is not null ? ContentDialogButton.Close : ContentDialogButton.Primary
        };
        dialog.Closing += (_, args) =>
        {
            if (args.Result == ContentDialogResult.None && close is null && buttons != DialogButtons.OK)
            {
                args.Cancel = true;
            }
        };
        return await dialog.ShowAsync() switch
        {
            ContentDialogResult.Primary => primary,
            ContentDialogResult.Secondary when secondary is { } result => result,
            ContentDialogResult.None when close is { } result => result,
            ContentDialogResult.None when buttons == DialogButtons.OK => DialogResult.OK,
            _ => throw new InvalidOperationException("The dialog closed without a valid button result.")
        };
    }

    private static string Label(DialogResult result) => result == DialogResult.TryAgain ? "Try Again" : result.ToString();
}

public interface IClipboardService
{
    void CopyText(string text);
    Task<string?> ReadTextAsync();
    void Clear();
    string DescribeFormats();
    void CopyImage(Uri source);
    Task<BitmapImage?> ReadImageAsync();
}

public sealed class ClipboardService : IClipboardService
{
    public void CopyText(string text)
    {
        var package = new DataPackage();
        package.SetText(text);
        Clipboard.SetContent(package);
        Clipboard.Flush();
    }

    public async Task<string?> ReadTextAsync()
    {
        var content = Clipboard.GetContent();
        return content.Contains(StandardDataFormats.Text) ? await content.GetTextAsync() : null;
    }

    public void Clear() => Clipboard.Clear();

    public string DescribeFormats()
    {
        var content = Clipboard.GetContent();
        return $"Clipboard contains:\n  Text: {content.Contains(StandardDataFormats.Text)}\n" +
            $"  Image: {content.Contains(StandardDataFormats.Bitmap)}\n" +
            $"  File Drop List: {content.Contains(StandardDataFormats.StorageItems)}\n" +
            $"  Audio: {content.AvailableFormats.Contains("WaveAudio")}";
    }

    public void CopyImage(Uri source)
    {
        var package = new DataPackage();
        package.SetBitmap(RandomAccessStreamReference.CreateFromUri(source));
        Clipboard.SetContent(package);
        Clipboard.Flush();
    }

    public async Task<BitmapImage?> ReadImageAsync()
    {
        var content = Clipboard.GetContent();
        if (!content.Contains(StandardDataFormats.Bitmap)) return null;
        var reference = await content.GetBitmapAsync();
        using var stream = await reference.OpenReadAsync();
        var image = new BitmapImage();
        await image.SetSourceAsync(stream);
        return image;
    }
}

public interface IFilePickerService
{
    Task<string?> OpenFileAsync();
    Task<IReadOnlyList<string>> OpenFilesAsync();
    Task<string?> SaveFileAsync(string content);
    Task<string?> OpenFolderAsync();
}

public sealed class FilePickerService : IFilePickerService
{
    private static FileOpenPicker CreateOpenPicker()
    {
        var picker = new FileOpenPicker(App.MainWindow.AppWindow.Id);
        picker.FileTypeFilter.Add("*");
        picker.FileTypeFilter.Add(".txt");
        return picker;
    }

    public async Task<string?> OpenFileAsync() => (await CreateOpenPicker().PickSingleFileAsync())?.Path;

    public async Task<IReadOnlyList<string>> OpenFilesAsync() =>
        (await CreateOpenPicker().PickMultipleFilesAsync()).Select(file => file.Path).ToArray();

    public async Task<string?> SaveFileAsync(string content)
    {
        var picker = new FileSavePicker(App.MainWindow.AppWindow.Id)
        {
            SuggestedFileName = "Sample.txt",
            DefaultFileExtension = ".txt"
        };
        picker.FileTypeChoices.Add("Text files", [".txt"]);
        var file = await picker.PickSaveFileAsync();
        if (file is null) return null;
        await File.WriteAllTextAsync(file.Path, content);
        return file.Path;
    }

    public async Task<string?> OpenFolderAsync() =>
        (await new FolderPicker(App.MainWindow.AppWindow.Id).PickSingleFolderAsync())?.Path;
}
