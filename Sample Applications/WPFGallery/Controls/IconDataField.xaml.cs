
namespace WPFGallery.Controls
{
    /// <summary>
    /// Interaction logic for IconDataField.xaml.
    /// Displays a single read-only icon metadata value (e.g. icon name, Unicode
    /// code point or glyph) together with a button that copies the value to the
    /// clipboard. The value is rendered by a TextBlock that lives in the
    /// control's content tree, so it is exposed to UI Automation as content and
    /// is reachable by Narrator scan mode.
    /// </summary>
    public partial class IconDataField : UserControl
    {
        public IconDataField()
        {
            InitializeComponent();
        }

        public string Value
        {
            get { return (string)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(nameof(Value), typeof(string), typeof(IconDataField), new PropertyMetadata(string.Empty));

        public string Label
        {
            get { return (string)GetValue(LabelProperty); }
            set { SetValue(LabelProperty, value); }
        }

        public static readonly DependencyProperty LabelProperty =
            DependencyProperty.Register(nameof(Label), typeof(string), typeof(IconDataField), new PropertyMetadata(string.Empty));

        private async void CopyButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(Value))
            {
                return;
            }

            try
            {
                App.Services.GetRequiredService<Services.IClipboardService>().CopyText(Value);
                CopyIcon.Symbol = Symbol.Accept;
                AutomationPeer peer = FrameworkElementAutomationPeer.CreatePeerForElement(this);
                peer?.RaiseNotificationEvent(
                    AutomationNotificationKind.Other,
                    AutomationNotificationProcessing.ImportantMostRecent,
                    $"{Label} Copied",
                    "ButtonClickedActivity"
                );
                await Task.Delay(2000);
                CopyIcon.Symbol = Symbol.Copy;
            }
            catch (Exception ex)
            {
                await App.Services.GetRequiredService<Services.IDialogService>().ShowAsync(XamlRoot,
                    "Error copying to clipboard: " + ex.Message, "Clipboard error", image: Services.DialogImage.Error);
            }
        }
    }
}
