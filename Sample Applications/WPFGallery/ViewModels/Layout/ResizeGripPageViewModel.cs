using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFGallery.ViewModels
{
    public partial class ResizeGripPageViewModel : ObservableObject
    {
        [ObservableProperty]
        public partial string PageTitle { get; set; } = "ResizeGrip";

        [ObservableProperty]
        public partial string PageDescription { get; set; } = "";

        public ResizeGripPageViewModel() { }
    }
}
