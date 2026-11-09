using WPFGallery.Services;
using WPFGallery.ViewModels;

namespace WPFGallery.Views;

public partial class FileAndFolderDialogsPage : Page
{
    public FileAndFolderDialogsPageViewModel ViewModel { get; }
    private readonly IFilePickerService _pickers;

    public FileAndFolderDialogsPage() : this(
        App.Services.GetRequiredService<FileAndFolderDialogsPageViewModel>(),
        App.Services.GetRequiredService<IFilePickerService>())
    { }

    public FileAndFolderDialogsPage(FileAndFolderDialogsPageViewModel viewModel, IFilePickerService pickers)
    {
        ViewModel = viewModel;
        _pickers = pickers;
        InitializeComponent();
    }

    private async void PickSingleFileButton_Click(object sender, RoutedEventArgs args)
    {
        if (await _pickers.OpenFileAsync() is { } path) ViewModel.SingleFilePath = path;
    }

    private async void PickMultipleFilesButton_Click(object sender, RoutedEventArgs args)
    {
        var paths = await _pickers.OpenFilesAsync();
        if (paths.Count > 0)
            ViewModel.MultipleFilesPath = $"Selected {paths.Count} file(s): {string.Join(", ", paths)}";
    }

    private async void SaveFileButton_Click(object sender, RoutedEventArgs args)
    {
        try
        {
            if (await _pickers.SaveFileAsync(ViewModel.FileContent) is { } path)
                ViewModel.SavedFilePath = $"File saved successfully: {path}";
        }
        catch (Exception ex)
        {
            ViewModel.SavedFilePath = $"Error saving file: {ex.Message}";
        }
    }

    private async void PickFolderButton_Click(object sender, RoutedEventArgs args)
    {
        if (await _pickers.OpenFolderAsync() is { } path) ViewModel.SelectedFolderPath = path;
    }
}
