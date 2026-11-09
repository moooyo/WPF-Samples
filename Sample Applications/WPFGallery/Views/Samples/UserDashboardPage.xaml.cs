using System.ComponentModel;
using WPFGallery.Controls;
using WPFGallery.Models;
using WPFGallery.Services;
using WPFGallery.ViewModels.Samples;

namespace WPFGallery.Views
{
    /// <summary>
    /// Interaction logic for UserDashboardPage.xaml
    /// </summary>
    public partial class UserDashboardPage : Page
    {
        public UserDashboardPageViewModel ViewModel { get; }

        public UserDashboardPage() : this(App.Services.GetRequiredService<UserDashboardPageViewModel>()) { }

        public UserDashboardPage(UserDashboardPageViewModel viewModel)
        {
            ViewModel = viewModel;
            DataContext = this;
            InitializeComponent();
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.EditUserStartCommand.Execute(null);
            save_button.Focus(FocusState.Programmatic);
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            string currentUserName = ViewModel.EditableUser?.Name ?? "";
            ViewModel.EditUserCommitCommand.Execute(null);

            NotifyStatus($"User {currentUserName} saved");

            edit_button.Focus(FocusState.Programmatic);
        }

        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel.SelectedUser is not User user) return;
            var result = await App.Services.GetRequiredService<IDialogService>().ShowAsync(
                XamlRoot, $"Delete user {user.Name}? This action cannot be undone.",
                "Delete user", DialogButtons.OKCancel, DialogImage.Warning, DialogResult.Cancel,
                primaryButtonText: "Delete");
            if (result != DialogResult.OK) return;
            await ViewModel.RemoveUserCommand.ExecuteAsync(user);

            NotifyStatus($"User {ViewModel.DeletedName} deleted");

            edit_button.Focus(FocusState.Programmatic);
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.EditUserCancelCommand.Execute(null);
            edit_button.Focus(FocusState.Programmatic);
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.NewSize.Width < 600)
            {
                Grid_SetProperties(UserListGrid, 0, 0, 1, 2);
                Grid_SetProperties(UserDetailsGrid, 1, 0, 1, 2);
                UserDetailsGrid.Margin = new Thickness(0);
                UserDetailFormGrid.Margin = new Thickness(20, 10, 20, 10);
                UserList.ClearValue(WidthProperty);
                NewUserButton.HorizontalAlignment = HorizontalAlignment.Right;
                Grid_SetProperties(FirstNamePanel, 0, 0, 2, 1);
                Grid_SetProperties(LastNamePanel, 0, 1, 2, 1);
                FirstNamePanel.Margin = new Thickness(0, 0, 10, 0);
                UserDetailHeader.Orientation = Orientation.Horizontal;
                UserDetailHeaderPanel.HorizontalAlignment = HorizontalAlignment.Left;
                UserDetailHeaderNameBox.HorizontalAlignment = HorizontalAlignment.Left;
                UserDetailHeaderCompanyBox.HorizontalAlignment = HorizontalAlignment.Left;
            }
            else if (e.NewSize.Width >= 400 && e.NewSize.Width < 800)
            {
                Grid_SetProperties(UserListGrid, 0, 0, 2, 1);
                Grid_SetProperties(UserDetailsGrid, 0, 1, 2, 1);
                UserDetailsGrid.Margin = new Thickness(-10, 0, -20, 0);
                UserDetailFormGrid.Margin = new Thickness(0, 10, 0, 10);
                UserList.Width = 240;
                NewUserButton.HorizontalAlignment = HorizontalAlignment.Center;
                Grid_SetProperties(FirstNamePanel, 0, 0, 1, 2);
                Grid_SetProperties(LastNamePanel, 1, 0, 1, 2);
                FirstNamePanel.Margin = new Thickness(0);
                UserDetailHeader.Orientation = Orientation.Vertical;
                UserDetailHeaderPanel.HorizontalAlignment = HorizontalAlignment.Center;
                UserDetailHeaderNameBox.HorizontalAlignment = HorizontalAlignment.Center;
                UserDetailHeaderCompanyBox.HorizontalAlignment = HorizontalAlignment.Center;
            }
            else
            {
                Grid_SetProperties(UserListGrid, 0, 0, 2, 1);
                Grid_SetProperties(UserDetailsGrid, 0, 1, 2, 1);
                UserDetailsGrid.Margin = new Thickness(0);
                UserDetailFormGrid.Margin = new Thickness(20, 10, 20, 10);
                UserList.Width = 300;
                NewUserButton.HorizontalAlignment = HorizontalAlignment.Center;
                Grid_SetProperties(FirstNamePanel, 0, 0, 2, 1);
                Grid_SetProperties(LastNamePanel, 0, 1, 2, 1);
                FirstNamePanel.Margin = new Thickness(0, 0, 10, 0);
                UserDetailHeader.Orientation = Orientation.Horizontal;
                UserDetailHeaderPanel.HorizontalAlignment = HorizontalAlignment.Left;
                UserDetailHeaderNameBox.HorizontalAlignment = HorizontalAlignment.Left;
                UserDetailHeaderCompanyBox.HorizontalAlignment = HorizontalAlignment.Left;
            }
        }

        private void Grid_SetProperties(FrameworkElement uiElement, int row, int column, int rowSpan, int columnSpan)
        {
            Grid.SetRow(uiElement, row);
            Grid.SetColumn(uiElement, column);
            Grid.SetRowSpan(uiElement, rowSpan);
            Grid.SetColumnSpan(uiElement, columnSpan);
        }

        private void AgeSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            if (sender is not Slider || !ViewModel.IsEditing)
                return;

            int newAge = (int)e.NewValue;

            NotifyStatus($"New age {newAge}");
        }

        private void NotifyStatus(string message)
        {
            // Save and delete can collapse the originating control before its notification is raised.
            if (FrameworkElementAutomationPeer.CreatePeerForElement(UserList) is AutomationPeer peer)
            {
                peer.RaiseNotificationEvent(
                    AutomationNotificationKind.Other,
                    AutomationNotificationProcessing.ImportantMostRecent,
                    message,
                    "UserDashboardActivity");
            }
        }
    }
}
