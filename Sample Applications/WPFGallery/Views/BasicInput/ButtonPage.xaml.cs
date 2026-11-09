

using WPFGallery.ViewModels;

namespace WPFGallery.Views
{
    /// <summary>
    /// Interaction logic for Button.xaml
    /// </summary>
    public partial class ButtonPage : Page
    {
        public ButtonPageViewModel ViewModel { get; }

        public ButtonPage() : this(App.Services.GetRequiredService<ButtonPageViewModel>()) { }

        public ButtonPage(ButtonPageViewModel viewModel)
        {
            ViewModel = viewModel;
            DataContext = this;
            InitializeComponent();
        }
    }
}
