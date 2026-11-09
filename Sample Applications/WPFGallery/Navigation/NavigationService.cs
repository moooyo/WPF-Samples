using Microsoft.UI.Xaml.Navigation;

namespace WPFGallery.Navigation;

public interface INavigationService
{
    void Navigate(Type type, bool adjustFocus = true);
    void NavigateTo(Type? type);
    void SetFrame(Frame frame);
    void NavigateBack();
    void NavigateForward();
    bool IsBackHistoryNonEmpty();
    event EventHandler<NavigatingEventArgs>? Navigating;
}

public sealed class NavigationService : INavigationService
{
    private Frame? _frame;
    private bool _adjustFocus;

    public event EventHandler<NavigatingEventArgs>? Navigating;

    public void SetFrame(Frame frame)
    {
        if (_frame is not null) _frame.Navigated -= OnNavigated;
        _frame = frame;
        _frame.Navigated += OnNavigated;
    }

    public void Navigate(Type type, bool adjustFocus = true)
    {
        ArgumentNullException.ThrowIfNull(type);
        var frame = GetFrame();
        if (frame.CurrentSourcePageType == type) return;
        _adjustFocus = adjustFocus;
        if (!frame.Navigate(type))
            throw new InvalidOperationException($"Navigation to {type.FullName} failed.");
    }

    public void NavigateTo(Type? type)
    {
        if (type is not null) Navigate(type);
    }

    public void NavigateBack()
    {
        var frame = GetFrame();
        if (frame.CanGoBack)
        {
            _adjustFocus = true;
            frame.GoBack();
        }
    }

    public void NavigateForward()
    {
        var frame = GetFrame();
        if (frame.CanGoForward)
        {
            _adjustFocus = true;
            frame.GoForward();
        }
    }

    public bool IsBackHistoryNonEmpty() => _frame?.CanGoBack == true;

    private Frame GetFrame() => _frame ?? throw new InvalidOperationException("The navigation frame is not initialized.");

    private void OnNavigated(object sender, NavigationEventArgs args)
    {
        Navigating?.Invoke(this, new NavigatingEventArgs(args.SourcePageType));
        if (_adjustFocus && args.Content is Page page)
        {
            page.Loaded += MoveFocusToPage;
        }
    }

    private static void MoveFocusToPage(object sender, RoutedEventArgs args)
    {
        var page = (Page)sender;
        page.Loaded -= MoveFocusToPage;
        if (FocusManager.FindFirstFocusableElement(page) is Control control)
            control.Focus(FocusState.Programmatic);
    }
}
