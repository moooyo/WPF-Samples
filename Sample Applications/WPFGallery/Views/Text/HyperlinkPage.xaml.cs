using System;
using System.Collections.Generic;
using System.Text;
using WPFGallery.ViewModels;

namespace WPFGallery.Views
{
    /// <summary>
    /// Interaction logic for HyperlinkPage.xaml
    /// </summary>
    public partial class HyperlinkPage : Page
    {
        public HyperlinkPage() : this(App.Services.GetRequiredService<HyperlinkPageViewModel>()) { }

        public HyperlinkPage(HyperlinkPageViewModel viewModel)
        {
            ViewModel = viewModel;
            DataContext = this;
            InitializeComponent();
        }

        public HyperlinkPageViewModel ViewModel { get; }

    }
}
