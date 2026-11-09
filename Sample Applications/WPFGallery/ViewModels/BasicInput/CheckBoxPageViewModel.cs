// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and WPF UI Contributors.
// All Rights Reserved.

namespace WPFGallery.ViewModels;

public partial class CheckBoxPageViewModel : ObservableObject
{
    [ObservableProperty]
    public partial string PageTitle { get; set; } = "CheckBox";

    [ObservableProperty]
    public partial string PageDescription { get; set; } = "";

    [ObservableProperty]
    public partial bool? SelectAllCheckBoxChecked { get; set; } = null;

    [ObservableProperty]
    public partial bool OptionOneCheckBoxChecked { get; set; } = false;

    [ObservableProperty]
    public partial bool OptionTwoCheckBoxChecked { get; set; } = true;

    [ObservableProperty]
    public partial bool OptionThreeCheckBoxChecked { get; set; } = false;

    [RelayCommand]
    private void OnSelectAllChecked(object sender)
    {
        if (sender is not CheckBox checkBox)
            return;

        if (checkBox.IsChecked == null)
            checkBox.IsChecked = !(
                OptionOneCheckBoxChecked && OptionTwoCheckBoxChecked && OptionThreeCheckBoxChecked
            );

        if (checkBox.IsChecked == true)
        {
            OptionOneCheckBoxChecked = true;
            OptionTwoCheckBoxChecked = true;
            OptionThreeCheckBoxChecked = true;
        }
        else if (checkBox.IsChecked == false)
        {
            OptionOneCheckBoxChecked = false;
            OptionTwoCheckBoxChecked = false;
            OptionThreeCheckBoxChecked = false;
        }
    }

    [RelayCommand]
    private void OnSingleChecked(string option)
    {
        if (OptionOneCheckBoxChecked && OptionTwoCheckBoxChecked && OptionThreeCheckBoxChecked)
            SelectAllCheckBoxChecked = true;
        else if (!OptionOneCheckBoxChecked && !OptionTwoCheckBoxChecked && !OptionThreeCheckBoxChecked)
            SelectAllCheckBoxChecked = false;
        else
            SelectAllCheckBoxChecked = null;
    }
}
