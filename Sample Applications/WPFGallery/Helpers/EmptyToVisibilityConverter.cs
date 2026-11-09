namespace WPFGallery.Helpers;

/// <summary>
/// Converts an empty string to Visibility.Collapsed
/// </summary>
internal sealed class EmptyToVisibilityConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, string language)
    {
        if (value is string str)
        {
            return string.IsNullOrWhiteSpace(str) ? Visibility.Collapsed : Visibility.Visible;
        }

        return value is null ? Visibility.Collapsed : Visibility.Visible;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, string language)
    {
        throw new NotImplementedException();
    }
}