
using WPFGallery.ViewModels;


namespace WPFGallery.Views
{
    /// <summary>
    /// Interaction logic for PasswordBoxPage.xaml
    /// </summary>
    public partial class PasswordBoxPage : Page
    {
        public PasswordBoxPageViewModel ViewModel { get; }
        public PasswordBoxPage() : this(App.Services.GetRequiredService<PasswordBoxPageViewModel>()) { }

        public PasswordBoxPage(PasswordBoxPageViewModel viewModel)
        {
            ViewModel = viewModel;
            DataContext = this;
            InitializeComponent();
        }
    }
}
