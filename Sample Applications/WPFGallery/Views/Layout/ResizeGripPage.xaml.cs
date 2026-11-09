using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFGallery.ViewModels;

namespace WPFGallery.Views
{
    /// <summary>
    /// Interaction logic for ResizeGripPage.xaml
    /// </summary>
    public partial class ResizeGripPage : Page
    {
        public ResizeGripPage() : this(App.Services.GetRequiredService<ResizeGripPageViewModel>()) { }

        public ResizeGripPage(ResizeGripPageViewModel viewModel)
        {
            ViewModel = viewModel;
            DataContext = this;
            InitializeComponent();
        }

        public ResizeGripPageViewModel ViewModel { get; }

        private void OpenResizeGripWindow_Click(object sender, RoutedEventArgs e)
        {
            var window = new WinUIEx.WindowEx()
            {
                Width = 500,
                Height = 300,
                IsResizable = true,
                Content = new TextBlock
                {
                    Text = "Drag the window edges or corners to resize this WinUI window.",
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    FontSize = 16
                }
            };
            App.ShowSampleWindow(window);
        }
    }
}
