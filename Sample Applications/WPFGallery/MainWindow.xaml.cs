using Microsoft.UI.Xaml.Navigation;
using WPFGallery.Helpers;
using WPFGallery.Models;
using WPFGallery.Navigation;
using WPFGallery.ViewModels;
using WPFGallery.Views;

namespace WPFGallery;

public partial class MainWindow : WinUIEx.WindowEx
{
    private readonly INavigationService _navigationService;
    private readonly Dictionary<Type, NavigationViewItem> _navigationItems = [];
    private bool _synchronizingSelection;

    public MainWindowViewModel ViewModel { get; }
    public ElementTheme RootTheme
    {
        get => Root.RequestedTheme;
        set => Root.RequestedTheme = value;
    }

    public MainWindow(MainWindowViewModel viewModel, INavigationService navigationService)
    {
        ViewModel = viewModel;
        _navigationService = navigationService;
        InitializeComponent();
        if (!Utility.IsBackdropDisabled())
        {
            SystemBackdrop = new MicaBackdrop();
        }
        foreach (var item in ViewModel.Controls)
        {
            Navigation.MenuItems.Add(CreateNavigationItem(item));
        }
        _navigationService.SetFrame(RootContentFrame);
        _navigationService.Navigating += OnNavigating;
        Root.Loaded += (_, _) =>
        {
            if (Navigation.SettingsItem is not NavigationViewItem settingsItem)
                throw new InvalidOperationException("The navigation settings item has not been initialized.");
            AutomationProperties.SetAutomationId(settingsItem, "NavSettings");
            _navigationService.Navigate(typeof(DashboardPage), false);
        };
        Closed += (_, _) => _navigationService.Navigating -= OnNavigating;
    }

    private NavigationViewItem CreateNavigationItem(ControlInfoDataItem data)
    {
        var item = new NavigationViewItem
        {
            Content = data.Title,
            Tag = data.PageType,
            Icon = new FontIcon { Glyph = data.IconGlyph ?? string.Empty }
        };
        AutomationProperties.SetName(item, data.Title);
        AutomationProperties.SetAutomationId(item, $"Nav{data.PageName}");
        _navigationItems.Add(data.PageType, item);
        foreach (var child in data.Items)
        {
            item.MenuItems.Add(CreateNavigationItem(child));
        }
        return item;
    }

    private void Navigation_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
    {
        if (_synchronizingSelection) return;
        if (args.IsSettingsSelected)
        {
            _navigationService.Navigate(typeof(SettingsPage));
        }
        else if (args.SelectedItemContainer?.Tag is Type pageType)
        {
            _navigationService.Navigate(pageType);
        }
    }

    private void OnNavigating(object? sender, NavigatingEventArgs args)
    {
        if (args.PageType is not Type pageType)
            throw new InvalidOperationException("Navigation did not provide a destination page.");
        _synchronizingSelection = true;
        try
        {
            if (pageType == typeof(SettingsPage))
            {
                Navigation.SelectedItem = Navigation.SettingsItem;
            }
            else if (_navigationItems.TryGetValue(pageType, out var item))
            {
                foreach (var data in ViewModel.GetNavigationItemHierarchyFromPageType(pageType))
                {
                    if (_navigationItems.TryGetValue(data.PageType, out var parent))
                        parent.IsExpanded = true;
                }
                Navigation.SelectedItem = item;
            }
        }
        finally
        {
            _synchronizingSelection = false;
        }
    }

    private void Navigation_BackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs args) =>
        _navigationService.NavigateBack();

    private void RootContentFrame_Navigated(object sender, NavigationEventArgs args)
    {
        if (args.Content is Page page)
            AutomationProperties.SetAutomationId(page, args.SourcePageType.Name);
        ViewModel.UpdateCanNavigateBack();
    }

    private void SearchBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
    {
        if (args.Reason != AutoSuggestionBoxTextChangeReason.UserInput) return;
        sender.ItemsSource = _navigationItems.Keys.Select(type =>
            ViewModel.GetNavigationItemHierarchyFromPageType(type).Last())
            .Where(item => item.Title.Contains(sender.Text, StringComparison.OrdinalIgnoreCase))
            .ToArray();
    }

    private void SearchBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        if (args.ChosenSuggestion is ControlInfoDataItem item)
        {
            _navigationService.Navigate(item.PageType);
        }
        else
        {
            ViewModel.UpdateSearchText(args.QueryText);
        }
    }
}
