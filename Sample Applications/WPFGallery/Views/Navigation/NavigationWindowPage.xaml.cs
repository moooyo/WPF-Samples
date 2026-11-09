using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFGallery.ViewModels;

namespace WPFGallery.Views
{
    /// <summary>
    /// Interaction logic for NavigationWindowPage.xaml
    /// </summary>
    public partial class NavigationWindowPage : Page
    {
        public NavigationWindowPage() : this(App.Services.GetRequiredService<NavigationWindowPageViewModel>()) { }

        public NavigationWindowPage(NavigationWindowPageViewModel viewModel)
        {
            ViewModel = viewModel;
            DataContext = this;
            InitializeComponent();
        }

        public NavigationWindowPageViewModel ViewModel { get; }

        private void OpenNavigationWindow_Click(object sender, RoutedEventArgs e)
        {
            var window = new FrameWindow { Title = "Navigation Window" };
            App.ShowSampleWindow(window);
        }
    }
}
