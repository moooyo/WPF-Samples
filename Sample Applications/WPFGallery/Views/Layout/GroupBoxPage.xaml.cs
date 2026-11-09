using System;
using System.Collections.Generic;
using System.Text;
using WPFGallery.ViewModels;

namespace WPFGallery.Views
{
    /// <summary>
    /// Interaction logic for GroupBoxPage.xaml
    /// </summary>
    public partial class GroupBoxPage : Page
    {
        public GroupBoxPage() : this(App.Services.GetRequiredService<GroupBoxPageViewModel>()) { }

        public GroupBoxPage(GroupBoxPageViewModel viewModel)
        {
            ViewModel = viewModel;
            DataContext = this;
            InitializeComponent();
        }

        public GroupBoxPageViewModel ViewModel { get; }
    }
}
