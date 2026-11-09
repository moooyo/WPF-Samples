
using WPFGallery.ViewModels;

namespace WPFGallery.Views;

/// <summary>
/// Interaction logic for DatePickerPage.xaml
/// </summary>
public partial class DatePickerPage : Page
{
    public DatePickerPageViewModel ViewModel { get; }

    public DatePickerPage() : this(App.Services.GetRequiredService<DatePickerPageViewModel>()) { }

    public DatePickerPage(DatePickerPageViewModel viewModel)
    {
        ViewModel = viewModel;
        DataContext = this;

        InitializeComponent();
    }
}
