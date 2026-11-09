using System;
using System.Collections.Generic;
using System.Text;

namespace WPFGallery.Views
{
    /// <summary>
    /// Interaction logic for FrameWindow.xaml
    /// </summary>
    public partial class FrameWindow : WinUIEx.WindowEx
    {
        public FrameWindow()
        {
            InitializeComponent();
            SampleFrame.Navigate(typeof(Page1));
        }

        private void Back_Click(object sender, RoutedEventArgs args)
        {
            if (SampleFrame.CanGoBack) SampleFrame.GoBack();
        }

        private void Forward_Click(object sender, RoutedEventArgs args)
        {
            if (SampleFrame.CanGoForward) SampleFrame.GoForward();
        }
    }
}
