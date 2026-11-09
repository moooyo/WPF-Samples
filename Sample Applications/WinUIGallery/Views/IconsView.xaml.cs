using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.ApplicationModel.DataTransfer;
using WPFGallery.ViewModels;

namespace WPFGallery.Views;

public sealed partial class IconsView : UserControl
{
    public IconsViewModel ViewModel { get; } = new();
    public IconsView() => InitializeComponent();

    private void Tag_Click(object sender, RoutedEventArgs args) =>
        ViewModel.SearchText = (string)((Button)sender).Tag;

    private void CopyGlyph_Click(object sender, RoutedEventArgs args)
    {
        if (ViewModel.SelectedIcon is null)
        {
            StatusText.Text = "Select an icon to copy.";
            return;
        }
        try
        {
            var data = new DataPackage();
            data.SetText(ViewModel.SelectedIcon.TextGlyph);
            Clipboard.SetContent(data);
            StatusText.Text = "Icon glyph copied.";
        }
        catch (System.Runtime.InteropServices.COMException exception)
        {
            StatusText.Text = $"Cannot copy icon: {exception.Message}";
        }
    }
}
