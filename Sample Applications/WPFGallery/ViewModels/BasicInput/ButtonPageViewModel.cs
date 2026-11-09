// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and WPF UI Contributors.
// All Rights Reserved.

namespace WPFGallery.ViewModels;

/// <summary>
/// Interaction logic for Button.xaml
/// </summary>
public partial class ButtonPageViewModel : ObservableObject
{
    [ObservableProperty]
    public partial string PageTitle { get; set; } = "Button";

    [ObservableProperty]
    public partial string PageDescription { get; set; } = "";

    [ObservableProperty]
    public partial string Message { get; set; } = "Hello World!";

    [ObservableProperty]
    public partial bool IsSimpleButtonEnabled { get; set; } = true;

    [ObservableProperty]
    public partial bool IsUiButtonEnabled { get; set; } = true;

    [RelayCommand]
    private void OnSimpleButtonCheckboxChecked(object sender)
    {
        if (sender is not CheckBox checkbox)
            return;

        IsSimpleButtonEnabled = !(checkbox?.IsChecked ?? false);
    }

    [RelayCommand]
    private void OnUiButtonCheckboxChecked(object sender)
    {
        if (sender is not CheckBox checkbox)
            return;

        IsUiButtonEnabled = !(checkbox?.IsChecked ?? false);
    }
}