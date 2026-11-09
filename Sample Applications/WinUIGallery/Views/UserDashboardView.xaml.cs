using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using WPFGallery.ViewModels;

namespace WPFGallery.Views;

public sealed partial class UserDashboardView : UserControl
{
    public UserDashboardViewModel ViewModel { get; } = new();
    public UserDashboardView() => InitializeComponent();

    private async void Delete_Click(object sender, RoutedEventArgs args)
    {
        if (!ViewModel.CanEdit) return;
        var dialog = new ContentDialog
        {
            XamlRoot = XamlRoot,
            Title = $"Delete {ViewModel.SelectedUser!.Name}?",
            Content = "This removes the user from the sample dashboard.",
            PrimaryButtonText = "Delete",
            CloseButtonText = "Cancel",
            DefaultButton = ContentDialogButton.Close
        };
        if (await dialog.ShowAsync() == ContentDialogResult.Primary)
        {
            ViewModel.RemoveCommand.Execute(null);
        }
    }
}
