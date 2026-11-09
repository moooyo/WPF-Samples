// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and WPF UI Contributors.
// All Rights Reserved.

namespace WPFGallery.ViewModels;

public partial class RadioButtonPageViewModel : ObservableObject
{
    [ObservableProperty]
    public partial string PageTitle { get; set; } = "RadioButton";

    [ObservableProperty]
    public partial string PageDescription { get; set; } = "";

    [ObservableProperty]
    public partial bool IsRadioButtonEnabled { get; set; } = true;

    [RelayCommand]
    private void OnRadioButtonCheckboxChecked(object sender)
    {
        if (sender is not CheckBox checkbox)
            return;

        IsRadioButtonEnabled = !(checkbox?.IsChecked ?? false);
    }
}