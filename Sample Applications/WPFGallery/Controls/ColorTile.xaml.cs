
namespace WPFGallery.Controls
{
    /// <summary>
    /// Interaction logic for ColorTile.xaml
    /// </summary>
    public partial class ColorTile : Control
    {
        public ColorTile()
        {
            DefaultStyleKey = typeof(ColorTile);
            CopyCommand = new AsyncRelayCommand(CopyAsync);
        }

        public IAsyncRelayCommand CopyCommand { get; }
        public string CopyStatus
        {
            get => (string)GetValue(CopyStatusProperty);
            set => SetValue(CopyStatusProperty, value);
        }
        public static readonly DependencyProperty CopyStatusProperty =
            DependencyProperty.Register(nameof(CopyStatus), typeof(string), typeof(ColorTile), new PropertyMetadata(""));
        public CornerRadius TileRadius
        {
            get { return (CornerRadius)GetValue(TileRadiusProperty); }
            set { SetValue(TileRadiusProperty, value); }
        }

        public static readonly DependencyProperty TileRadiusProperty =
            DependencyProperty.Register("TileRadius", typeof(CornerRadius), typeof(ColorTile), new PropertyMetadata(new CornerRadius(0)));

        public string ColorName
        {
            get { return (string)GetValue(ColorNameProperty); }
            set { SetValue(ColorNameProperty, value); }
        }
        public static readonly DependencyProperty ColorNameProperty =
            DependencyProperty.Register("ColorName", typeof(string), typeof(ColorTile), new PropertyMetadata(""));

        public string ColorExplanation
        {
            get { return (string)GetValue(ColorExplanationProperty); }
            set { SetValue(ColorExplanationProperty, value); }
        }
        public static readonly DependencyProperty ColorExplanationProperty =
            DependencyProperty.Register("ColorExplanation", typeof(string), typeof(ColorTile), new PropertyMetadata(""));

        public string ColorBrushName
        {
            get { return (string)GetValue(ColorBrushNameProperty); }
            set { SetValue(ColorBrushNameProperty, value); }
        }
        public static readonly DependencyProperty ColorBrushNameProperty =
            DependencyProperty.Register("ColorBrushName", typeof(string), typeof(ColorTile), new PropertyMetadata(""));

        public string ColorValue
        {
            get { return (string)GetValue(ColorValueProperty); }
            set { SetValue(ColorValueProperty, value); }
        }
        public static readonly DependencyProperty ColorValueProperty =
            DependencyProperty.Register("ColorValue", typeof(string), typeof(ColorTile), new PropertyMetadata(""));

        public bool ShowSeparator
        {
            get { return (bool)GetValue(ShowSeparatorProperty); }
            set { SetValue(ShowSeparatorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShowSeparator.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowSeparatorProperty =
            DependencyProperty.Register("ShowSeparator", typeof(bool), typeof(ColorTile), new PropertyMetadata(true));


        public bool ShowWarning
        {
            get { return (bool)GetValue(ShowWarningProperty); }
            set { SetValue(ShowWarningProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShowSeparator.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowWarningProperty =
            DependencyProperty.Register("ShowWarning", typeof(bool), typeof(ColorTile), new PropertyMetadata(false));

        private async Task CopyAsync()
        {
            if (string.IsNullOrEmpty(ColorBrushName)) return;
            try
            {
                App.Services.GetRequiredService<Services.IClipboardService>().CopyText(ColorBrushName);
                CopyStatus = "Color Brush Name Copied";
            }
            catch (Exception ex)
            {
                await App.Services.GetRequiredService<Services.IDialogService>().ShowAsync(XamlRoot,
                    "Error copying to clipboard: " + ex.Message, "Clipboard error", image: Services.DialogImage.Error);
            }
        }
    }
}
