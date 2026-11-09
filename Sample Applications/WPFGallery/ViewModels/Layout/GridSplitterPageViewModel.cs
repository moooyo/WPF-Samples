using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFGallery.ViewModels.Layout
{
    public partial class GridSplitterPageViewModel : ObservableObject
    {
        [ObservableProperty]
        public partial string PageTitle { get; set; } = "GridSplitter";

        [ObservableProperty]
        public partial string PageDescription { get; set; } = "";

        public GridSplitterPageViewModel() { }
    }
}
