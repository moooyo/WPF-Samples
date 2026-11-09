using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFGallery.ViewModels;

namespace WPFGallery.Views
{
    /// <summary>
    /// Interaction logic for FramePage.xaml
    /// </summary>
    public partial class FramePage : Page
    {
        public FramePage() : this(App.Services.GetRequiredService<FramePageViewModel>()) { }

        public FramePage(FramePageViewModel viewModel)
        {
            ViewModel = viewModel;
            DataContext = this;
            InitializeComponent();
        }

        public FramePageViewModel ViewModel { get; }

        private void OpenFrameWindow_Click(object sender, RoutedEventArgs e)
        {
            FrameWindow window = new FrameWindow();
            App.ShowSampleWindow(window);
        }
    }
}
