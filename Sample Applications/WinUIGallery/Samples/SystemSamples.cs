using System.Runtime.InteropServices;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.Windows.Storage.Pickers;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage.Streams;

namespace WPFGallery.Samples;

internal static partial class SampleFactory
{
    private static Button ActionButton(string label, string id, TextBlock status, Func<Task> action)
    {
        var button = Identify(new Button { Content = label }, id, label);
        button.Click += async (_, _) =>
        {
            button.IsEnabled = false;
            try
            {
                await action();
            }
            catch (Exception exception) when (exception is COMException or IOException or UnauthorizedAccessException)
            {
                status.Text = $"Operation failed: {exception.Message}";
            }
            finally
            {
                button.IsEnabled = true;
            }
        };
        return button;
    }

    private static void FileDialogs(StackPanel target)
    {
        Note(target, "Desktop pickers are owned by the current AppWindow. Cancellation leaves files unchanged.");
        var status = Status("PickerResult");
        var content = new TextBox { Header = "Content to save", Text = "Hello from WinUI Gallery.", AcceptsReturn = true, Height = 100 };
        var panel = new StackPanel { Spacing = 12 };
        panel.Children.Add(ActionButton("Pick single file", "PickSingleFile", status, async () =>
        {
            var picker = new FileOpenPicker(App.Window.AppWindow.Id) { Title = "Select a file" };
            picker.FileTypeFilter.Add("*");
            var result = await picker.PickSingleFileAsync();
            status.Text = result is null ? "File selection cancelled." : $"Selected: {result.Path}";
        }));
        panel.Children.Add(ActionButton("Pick multiple files", "PickMultipleFiles", status, async () =>
        {
            var picker = new FileOpenPicker(App.Window.AppWindow.Id) { Title = "Select multiple files" };
            picker.FileTypeFilter.Add("*");
            var results = await picker.PickMultipleFilesAsync();
            status.Text = results.Count == 0 ? "File selection cancelled."
                : $"Selected {results.Count} file(s): {string.Join(", ", results.Select(file => file.Path))}";
        }));
        panel.Children.Add(content);
        panel.Children.Add(ActionButton("Save file", "SaveFile", status, async () =>
        {
            var picker = new FileSavePicker(App.Window.AppWindow.Id) { SuggestedFileName = "GallerySample", Title = "Save file" };
            picker.FileTypeChoices.Add("Text files", new List<string> { ".txt" });
            var result = await picker.PickSaveFileAsync();
            if (result is null)
            {
                status.Text = "Save cancelled.";
                return;
            }
            await File.WriteAllTextAsync(result.Path, content.Text);
            status.Text = $"File saved: {result.Path}";
        }));
        panel.Children.Add(ActionButton("Pick folder", "PickFolder", status, async () =>
        {
            var picker = new FolderPicker(App.Window.AppWindow.Id) { Title = "Select a folder" };
            var result = await picker.PickSingleFolderAsync();
            status.Text = result is null ? "Folder selection cancelled." : $"Selected folder: {result.Path}";
        }));
        panel.Children.Add(status);
        Add(target, "Open, save, and folder pickers", panel, """
            using Microsoft.Windows.Storage.Pickers;
            var picker = new FileOpenPicker(App.Window.AppWindow.Id);
            picker.FileTypeFilter.Add("*");
            var result = await picker.PickSingleFileAsync();
            if (result is not null) selectedPath = result.Path;
            """);
    }

    private static void Dialogs(StackPanel target)
    {
        Note(target, "ContentDialog replaces MessageBox. It is associated with the page's XamlRoot and supports up to three actions.");
        var status = Status("DialogResult");
        var panel = new StackPanel { Spacing = 12 };
        var buttons = new ComboBox
        {
            Header = "Dialog buttons",
            ItemsSource = new[] { "OK", "OK / Cancel", "Abort / Retry / Ignore", "Yes / No / Cancel", "Yes / No", "Retry / Cancel", "Cancel / Try / Continue" },
            SelectedIndex = 0
        };
        var severity = new ComboBox
        {
            Header = "Message type",
            ItemsSource = new[] { "None", "Error", "Question", "Warning", "Information" },
            SelectedIndex = 0
        };
        string[][] labels =
        [
            ["OK"], ["OK", "Cancel"], ["Abort", "Retry", "Ignore"], ["Yes", "No", "Cancel"],
            ["Yes", "No"], ["Retry", "Cancel"], ["Cancel", "Try", "Continue"]
        ];
        async Task Show(string title, string message, string[] actions,
            ContentDialogButton defaultButton = ContentDialogButton.Primary, InfoBarSeverity? messageType = null)
        {
            var dialog = new ContentDialog
            {
                XamlRoot = panel.XamlRoot,
                Title = title,
                Content = messageType is InfoBarSeverity kind
                    ? new InfoBar { IsOpen = true, IsClosable = false, Severity = kind, Message = message }
                    : message,
                PrimaryButtonText = actions[0],
                SecondaryButtonText = actions.Length == 3 ? actions[1] : "",
                CloseButtonText = actions.Length > 1 ? actions[^1] : "",
                DefaultButton = defaultButton
            };
            ContentDialogResult result = await dialog.ShowAsync();
            status.Text = $"Result: {result switch
            {
                ContentDialogResult.Primary => actions[0],
                ContentDialogResult.Secondary => actions[1],
                _ => actions.Length > 1 ? actions[^1] : "Dismissed"
            }}";
        }
        panel.Children.Add(ActionButton("Show simple message", "ShowSimpleDialog", status,
            () => Show("Message", "This is a simple message dialog.", ["OK"])));
        panel.Children.Add(ActionButton("Show custom title", "ShowCustomDialog", status,
            () => Show("Custom title", "A detailed description of what happened or what action is needed.", ["OK"])));
        panel.Children.Add(buttons);
        panel.Children.Add(ActionButton("Show selected buttons", "ShowButtonDialog", status,
            () => Show("Choose an action", "Select one of the available actions.", labels[buttons.SelectedIndex])));
        panel.Children.Add(severity);
        panel.Children.Add(ActionButton("Show selected message type", "ShowSeverityDialog", status,
            () => Show(severity.SelectedItem.ToString()!, $"This is a {severity.SelectedItem} message.", ["OK"],
                messageType: severity.SelectedIndex switch
                {
                    1 => InfoBarSeverity.Error,
                    3 => InfoBarSeverity.Warning,
                    4 => InfoBarSeverity.Informational,
                    _ => null
                })));
        panel.Children.Add(ActionButton("Information", "ShowInformation", status,
            () => Show("Information", "The operation completed successfully.", ["OK"], messageType: InfoBarSeverity.Informational)));
        panel.Children.Add(ActionButton("Error", "ShowError", status,
            () => Show("Error", "The operation could not be completed.", ["OK"], messageType: InfoBarSeverity.Error)));
        panel.Children.Add(ActionButton("Warning", "ShowWarning", status,
            () => Show("Warning", "This sample demonstrates a warning. Continue?", ["Continue", "Cancel"], messageType: InfoBarSeverity.Warning)));
        panel.Children.Add(ActionButton("Custom default button", "ShowDefaultDialog", status,
            () => Show("Save changes?", "The default action is No.", ["Yes", "No", "Cancel"], ContentDialogButton.Secondary)));
        panel.Children.Add(status);
        Add(target, "ContentDialog messages and decisions", panel, """
            var dialog = new ContentDialog
            {
                XamlRoot = XamlRoot,
                Title = "Save changes?",
                Content = "Choose whether to save.",
                PrimaryButtonText = "Yes",
                SecondaryButtonText = "No",
                CloseButtonText = "Cancel",
                DefaultButton = ContentDialogButton.Secondary
            };
            ContentDialogResult result = await dialog.ShowAsync();
            """);
    }

    private static void ClipboardSamples(StackPanel target)
    {
        var panel = new StackPanel { Spacing = 12 };
        var input = Identify(new TextBox { Header = "Text to copy", Text = "Hello from WinUI Gallery." }, "ClipboardInput", "Text to copy");
        var output = Identify(new TextBox { Header = "Pasted text", IsReadOnly = true }, "ClipboardOutput", "Pasted text");
        var status = Status("ClipboardResult");
        var image = Identify(new Image { MaxHeight = 200, HorizontalAlignment = HorizontalAlignment.Left }, "ClipboardImage", "Pasted image");
        panel.Children.Add(input);
        panel.Children.Add(ActionButton("Copy text", "CopyText", status, () =>
        {
            if (input.Text.Length == 0)
            {
                status.Text = "Nothing to copy - text box is empty.";
                return Task.CompletedTask;
            }
            var data = new DataPackage();
            data.SetText(input.Text);
            Clipboard.SetContent(data);
            status.Text = "Text copied.";
            return Task.CompletedTask;
        }));
        panel.Children.Add(ActionButton("Paste text", "PasteText", status, async () =>
        {
            DataPackageView data = Clipboard.GetContent();
            output.Text = data.Contains(StandardDataFormats.Text) ? await data.GetTextAsync() : "(No text in clipboard)";
            status.Text = "Clipboard text checked.";
        }));
        panel.Children.Add(output);
        panel.Children.Add(ActionButton("Clear clipboard", "ClearClipboard", status, () =>
        {
            Clipboard.Clear();
            output.Text = "";
            image.Source = null;
            status.Text = "Clipboard cleared.";
            return Task.CompletedTask;
        }));
        panel.Children.Add(ActionButton("Check data formats", "CheckClipboardFormats", status, () =>
        {
            DataPackageView data = Clipboard.GetContent();
            status.Text = $"Text: {data.Contains(StandardDataFormats.Text)}; Image: {data.Contains(StandardDataFormats.Bitmap)}; "
                + $"Files: {data.Contains(StandardDataFormats.StorageItems)}; Formats: {string.Join(", ", data.AvailableFormats)}";
            return Task.CompletedTask;
        }));
        panel.Children.Add(ActionButton("Copy sample image", "CopyImage", status, () =>
        {
            var data = new DataPackage();
            data.SetBitmap(RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/Gallery/win11-dashboard.png")));
            Clipboard.SetContent(data);
            status.Text = "Image copied.";
            return Task.CompletedTask;
        }));
        panel.Children.Add(ActionButton("Paste image", "PasteImage", status, async () =>
        {
            DataPackageView data = Clipboard.GetContent();
            if (!data.Contains(StandardDataFormats.Bitmap))
            {
                image.Source = null;
                status.Text = "No image in clipboard.";
                return;
            }
            var reference = await data.GetBitmapAsync();
            using var stream = await reference.OpenReadAsync();
            var bitmap = new BitmapImage();
            await bitmap.SetSourceAsync(stream);
            image.Source = bitmap;
            status.Text = $"Image pasted: {bitmap.PixelWidth} x {bitmap.PixelHeight}";
        }));
        panel.Children.Add(image);
        panel.Children.Add(status);
        Add(target, "Clipboard text, formats, and images", panel, """
            var data = new DataPackage();
            data.SetText(text);
            Clipboard.SetContent(data);
            var content = Clipboard.GetContent();
            if (content.Contains(StandardDataFormats.Text))
                text = await content.GetTextAsync();
            """);
    }
}
