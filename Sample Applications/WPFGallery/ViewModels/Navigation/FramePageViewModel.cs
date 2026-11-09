using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFGallery.ViewModels
{
    public partial class FramePageViewModel : ObservableObject
    {
        [ObservableProperty]
        public partial string PageTitle { get; set; } = "Frame";

        [ObservableProperty]
        public partial string PageDescription { get; set; } = "";

        public FramePageViewModel() { }
    }
}
