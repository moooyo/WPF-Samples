using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Automation;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using WPFGallery.Models;
using WPFGallery.Views;

namespace WPFGallery;

public sealed partial class MainPage : Page
{
    private readonly Dictionary<string, NavigationViewItem> _navigationItems = [];
    private bool _syncingSelection;
    public static MainPage Current { get; private set; } = null!;

    public MainPage()
    {
        InitializeComponent();
        Current = this;
        foreach (ControlInfoDataItem group in GalleryCatalog.Groups)
        {
            var item = CreateNavigationItem(group);
            foreach (ControlInfoDataItem sample in group.Items)
            {
                item.MenuItems.Add(CreateNavigationItem(sample));
            }
            Navigation.MenuItems.Add(item);
        }
        Navigation.Loaded += (_, _) =>
        {
            if (Navigation.SettingsItem is DependencyObject settings)
            {
                AutomationProperties.SetAutomationId(settings, "NavSettingsPage");
            }
        };
        Navigate(GalleryCatalog.Find("DashboardPage"));
    }

    private NavigationViewItem CreateNavigationItem(ControlInfoDataItem item)
    {
        var navigationItem = new NavigationViewItem { Content = item.Title, Tag = item };
        if (item.IconGlyph.Length > 0)
        {
            navigationItem.Icon = new FontIcon { Glyph = item.IconGlyph };
        }
        AutomationProperties.SetAutomationId(navigationItem, $"Nav{item.PageName}");
        _navigationItems.Add(item.PageName, navigationItem);
        return navigationItem;
    }

    public void Navigate(ControlInfoDataItem item)
    {
        if (ContentFrame.Content is GalleryPage page && page.Route == item.PageName)
        {
            return;
        }
        ContentFrame.Navigate(typeof(GalleryPage), item.PageName);
    }

    private void Navigation_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
    {
        if (_syncingSelection)
        {
            return;
        }
        if (args.IsSettingsSelected)
        {
            Navigate(GalleryCatalog.Settings);
        }
        else if (args.SelectedItemContainer?.Tag is ControlInfoDataItem item)
        {
            Navigate(item);
        }
    }

    private void Navigation_BackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs args)
    {
        if (ContentFrame.CanGoBack)
        {
            ContentFrame.GoBack();
        }
    }

    private void ContentFrame_Navigated(object sender, NavigationEventArgs args)
    {
        Navigation.IsBackEnabled = ContentFrame.CanGoBack;
        string route = (string)args.Parameter;
        _syncingSelection = true;
        try
        {
            if (route == "SettingsPage")
            {
                Navigation.SelectedItem = Navigation.SettingsItem;
            }
            else
            {
                foreach (ControlInfoDataItem group in GalleryCatalog.Groups.Where(group =>
                    group.Items.Any(item => item.PageName == route)))
                {
                    _navigationItems[group.PageName].IsExpanded = true;
                }
                Navigation.SelectedItem = _navigationItems[route];
            }
        }
        finally
        {
            _syncingSelection = false;
        }
    }

    private void Search_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
    {
        if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
        {
            sender.ItemsSource = GalleryCatalog.All.Where(item =>
                item.Title.Contains(sender.Text, StringComparison.OrdinalIgnoreCase)
                || item.Description.Contains(sender.Text, StringComparison.OrdinalIgnoreCase)).ToArray();
        }
    }

    private void Search_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        ControlInfoDataItem? item = args.ChosenSuggestion as ControlInfoDataItem
            ?? GalleryCatalog.All.FirstOrDefault(item =>
                item.Title.Equals(args.QueryText, StringComparison.OrdinalIgnoreCase));
        if (item is not null)
        {
            Navigate(item);
            sender.Text = "";
        }
    }
}
