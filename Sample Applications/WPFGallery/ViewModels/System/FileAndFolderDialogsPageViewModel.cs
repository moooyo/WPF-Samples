// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and WPF UI Contributors.
// All Rights Reserved.

namespace WPFGallery.ViewModels;

/// <summary>
/// Interaction logic for FileAndFolderDialogsPage.xaml
/// </summary>
public partial class FileAndFolderDialogsPageViewModel : ObservableObject
{
    [ObservableProperty]
    public partial string PageTitle { get; set; } = "File and Folder Dialogs";

    [ObservableProperty]
    public partial string PageDescription { get; set; } = "Use the OpenFileDialog, SaveFileDialog, and OpenFolderDialog to let users select files and folders in a secure way.";

    [ObservableProperty]
    public partial string SingleFilePath { get; set; } = "No file selected";

    [ObservableProperty]
    public partial string MultipleFilesPath { get; set; } = "No files selected";

    [ObservableProperty]
    public partial string FileContent { get; set; } = "Enter text here to save to a file...";

    [ObservableProperty]
    public partial string SavedFilePath { get; set; } = "No file saved";

    [ObservableProperty]
    public partial string SelectedFolderPath { get; set; } = "No folder selected";
}
