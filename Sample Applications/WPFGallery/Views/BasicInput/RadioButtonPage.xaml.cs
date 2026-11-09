
using WPFGallery.ViewModels;

namespace WPFGallery.Views;

/// <summary>
/// Interaction logic for RadioButtonPage.xaml
/// </summary>
public partial class RadioButtonPage : Page
{
    public RadioButtonPageViewModel ViewModel { get; }

    public RadioButtonPage() : this(App.Services.GetRequiredService<RadioButtonPageViewModel>()) { }

    public RadioButtonPage(RadioButtonPageViewModel viewModel)
    {
        ViewModel = viewModel;
        DataContext = this;

        InitializeComponent();
    }

    private void RadioButton_GotKeyboardFocus(object sender, RoutedEventArgs e)
    {
        var radioButton = sender as RadioButton;
        if (radioButton != null)
        {
            radioButton.IsChecked = true;
        }
    }
}