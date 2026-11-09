using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

namespace WPFGallery.Views;

public sealed partial class SampleNavigationPage : Page
{
    public int Number { get; private set; }
    public SampleNavigationPage() => InitializeComponent();

    protected override void OnNavigatedTo(NavigationEventArgs args)
    {
        base.OnNavigatedTo(args);
        Number = (int)args.Parameter;
        NumberText.Text = $"Page {Number}";
    }
}
