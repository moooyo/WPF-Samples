using WPFGallery.Services;

using WPFGallery.ViewModels;

namespace WPFGallery.Views
{
    /// <summary>
    /// Interaction logic for MessageBoxPage.xaml
    /// </summary>
    public partial class MessageBoxPage : Page
    {
        public MessageBoxPageViewModel ViewModel { get; }

        public MessageBoxPage() : this(App.Services.GetRequiredService<MessageBoxPageViewModel>()) { }

        public MessageBoxPage(MessageBoxPageViewModel viewModel)
        {
            ViewModel = viewModel;
            DataContext = this;
            InitializeComponent();
        }

        private async void ShowDefaultMessageButton_Click(object sender, RoutedEventArgs e)
        {
            var result = await App.Services.GetRequiredService<IDialogService>().ShowAsync(XamlRoot, "This is a simple message box!");
            ViewModel.DefaultMessageResult = $"Result: {result}";
        }

        private async void ShowCustomTitleButton_Click(object sender, RoutedEventArgs e)
        {
            var result = await App.Services.GetRequiredService<IDialogService>().ShowAsync(XamlRoot, "This is a detailed description of what happened or what action is needed.", "Custom Title");
            ViewModel.CustomTitleResult = $"Result: {result}";
        }

        private async void ShowButtonFromComboBox_Click(object sender, RoutedEventArgs e)
        {
            DialogButtons buttonType = ViewModel.SelectedButtonIndex switch
            {
                0 => DialogButtons.OK,
                1 => DialogButtons.OKCancel,
                2 => DialogButtons.AbortRetryIgnore,
                3 => DialogButtons.YesNoCancel,
                4 => DialogButtons.YesNo,
                5 => DialogButtons.RetryCancel,
                6 => DialogButtons.CancelTryContinue,
                _ => DialogButtons.OK
            };

            string buttonName = ViewModel.SelectedButtonIndex switch
            {
                0 => "OK",
                1 => "OK/Cancel",
                2 => "Abort/Retry/Ignore",
                3 => "Yes/No/Cancel",
                4 => "Yes/No",
                5 => "Retry/Cancel",
                6 => "Cancel/Try/Continue",
                _ => "OK"
            };

            var result = await App.Services.GetRequiredService<IDialogService>().ShowAsync(XamlRoot, $"This MessageBox has {buttonName} button(s).", $"{buttonName} Button(s)", buttonType);
            ViewModel.DifferentButtonsResult = $"Result: {result}";
        }

        private async void ShowImageFromComboBox_Click(object sender, RoutedEventArgs e)
        {
            DialogImage imageType = ViewModel.SelectedImageIndex switch
            {
                0 => DialogImage.None,
                1 => DialogImage.Error,
                2 => DialogImage.Question,
                3 => DialogImage.Warning,
                4 => DialogImage.Information,
                _ => DialogImage.None
            };

            string imageName = ViewModel.SelectedImageIndex switch
            {
                0 => "None",
                1 => "Error",
                2 => "Question",
                3 => "Warning",
                4 => "Information",
                _ => "None"
            };

            var result = await App.Services.GetRequiredService<IDialogService>().ShowAsync(XamlRoot, $"This MessageBox displays the {imageName} icon.", $"{imageName} Icon", DialogButtons.OK, imageType);
            ViewModel.DifferentImagesResult = $"Result: {result}";
        }

        // 6. Common Messages (Information, Error, Warning)
        private async void ShowCommonInformation_Click(object sender, RoutedEventArgs e)
        {
            var result = await App.Services.GetRequiredService<IDialogService>().ShowAsync(XamlRoot, "The operation completed successfully.", "Information", DialogButtons.OK, DialogImage.Information);
            ViewModel.CommonMessagesResult = $"Type: Information | Result: {result}";
        }

        private async void ShowCommonError_Click(object sender, RoutedEventArgs e)
        {
            var result = await App.Services.GetRequiredService<IDialogService>().ShowAsync(XamlRoot, "An error occurred! The operation could not be completed.", "Error", DialogButtons.OK, DialogImage.Error);
            ViewModel.CommonMessagesResult = $"Type: Error | Result: {result}";
        }

        private async void ShowCommonWarning_Click(object sender, RoutedEventArgs e)
        {
            var result = await App.Services.GetRequiredService<IDialogService>().ShowAsync(XamlRoot, "This action cannot be undone! Do you want to continue?", "Warning", DialogButtons.OKCancel, DialogImage.Warning);
            ViewModel.CommonMessagesResult = $"Type: Warning | Result: {result}";
        }

        // 7. Custom Default Button
        private async void ShowCustomDefaultButton_Click(object sender, RoutedEventArgs e)
        {
            var result = await App.Services.GetRequiredService<IDialogService>().ShowAsync(XamlRoot, "Do you want to save changes? Press Enter to select the default 'No' button.", "Save Changes", DialogButtons.YesNoCancel, DialogImage.Question, DialogResult.No);
            ViewModel.CustomDefaultResult = $"User selected: {result}";
        }
    }
}
