// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and WPF UI Contributors.
// All Rights Reserved.

namespace WPFGallery.ViewModels;

public partial class ComboBoxPageViewModel : ObservableObject
{
    [ObservableProperty]
    public partial string PageTitle { get; set; } = "ComboBox";

    [ObservableProperty]
    public partial string PageDescription { get; set; } = "";

    [ObservableProperty]
    public partial IList<string> ComboBoxFontFamilies { get; set; } = new ObservableCollection<string>
    {
        "Arial",
        "Comic Sans MS",
        "Segoe UI",
        "Times New Roman"
    };

    [ObservableProperty]
    public partial IList<int> ComboBoxFontSizes { get; set; } = new ObservableCollection<int>
    {
        8,
        9,
        10,
        11,
        12,
        14,
        16,
        18,
        20,
        24,
        28,
        36,
        48,
        72
    };
}
