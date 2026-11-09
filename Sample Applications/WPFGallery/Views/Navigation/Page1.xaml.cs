using System;
using System.Collections.Generic;
using System.Text;

namespace WPFGallery.Views
{
    /// <summary>
    /// Interaction logic for FramePage1.xaml
    /// </summary>
    public partial class Page1 : Page
    {
        public Page1()
        {
            InitializeComponent();
        }

        private void NavigateToPage2(Microsoft.UI.Xaml.Documents.Hyperlink sender,
            Microsoft.UI.Xaml.Documents.HyperlinkClickEventArgs args) => Frame.Navigate(typeof(Page2));
    }
}
