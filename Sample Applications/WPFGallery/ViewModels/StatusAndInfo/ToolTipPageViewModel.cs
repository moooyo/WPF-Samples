// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and WPF UI Contributors.
// All Rights Reserved.

namespace WPFGallery.ViewModels;

public partial class ToolTipPageViewModel : ObservableObject
{
    [ObservableProperty]
    public partial string PageTitle { get; set; } = "ToolTip";

    [ObservableProperty]
    public partial string PageDescription { get; set; } = "";


}

