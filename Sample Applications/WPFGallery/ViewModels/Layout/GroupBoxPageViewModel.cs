using System;
using System.Collections.Generic;
using System.Text;

namespace WPFGallery.ViewModels
{
    public partial class GroupBoxPageViewModel : ObservableObject
    {
        [ObservableProperty]
        public partial string PageTitle { get; set; } = "GroupBox";

        [ObservableProperty]
        public partial string PageDescription { get; set; } = "";

        public GroupBoxPageViewModel() { }
    }
}
