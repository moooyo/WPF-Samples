using System;
using System.Collections.Generic;
using System.Text;
using WPFGallery.ViewModels;
using WPFGallery.ViewModels.Layout;

namespace WPFGallery.Views
{
    /// <summary>
    /// Interaction logic for GridSplitterPage.xaml
    /// </summary>
    public partial class GridSplitterPage : Page
    {
        public GridSplitterPage() : this(App.Services.GetRequiredService<GridSplitterPageViewModel>()) { }

        public GridSplitterPage(GridSplitterPageViewModel viewModel)
        {
            ViewModel = viewModel;
            DataContext = this;
            InitializeComponent();
        }

        public GridSplitterPageViewModel ViewModel { get; }

    }
}
