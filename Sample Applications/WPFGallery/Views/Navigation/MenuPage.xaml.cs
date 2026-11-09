
using WPFGallery.ViewModels;

namespace WPFGallery.Views
{
    /// <summary>
    /// Interaction logic for MenuPage.xaml
    /// </summary>
    public partial class MenuPage : Page
    {
        public MenuPageViewModel ViewModel { get; }

        public MenuPage() : this(App.Services.GetRequiredService<MenuPageViewModel>()) { }

        public MenuPage(MenuPageViewModel viewModel)
        {
            ViewModel = viewModel;
            DataContext = this;

            InitializeComponent();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuFlyoutItem menuItem)
            {
                StatusMenuItem.Text = $"You pressed {menuItem.Text}";
            }
        }
    }
}
