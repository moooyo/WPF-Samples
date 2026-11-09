using System;
using System.Collections.Generic;
using System.Text;

namespace WPFGallery.ViewModels
{
    public partial class HyperlinkPageViewModel : ObservableObject
    {
        [ObservableProperty]
        public partial string PageTitle { get; set; } = "Hyperlink";

        [ObservableProperty]
        public partial string PageDescription { get; set; } = "";

        public HyperlinkPageViewModel() { }
    }
}
