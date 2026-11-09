using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Automation;
using Microsoft.UI.Xaml.Controls;
using WPFGallery.Controls;

namespace WPFGallery.Samples;

internal static partial class SampleFactory
{
    public static void Populate(string route, StackPanel target)
    {
        target.Children.Clear();
        switch (route)
        {
            case "ButtonPage": Buttons(target); break;
            case "CheckBoxPage": CheckBoxes(target); break;
            case "ComboBoxPage": ComboBoxes(target); break;
            case "RadioButtonPage": RadioButtons(target); break;
            case "SliderPage": Sliders(target); break;
            case "DataGridPage": Products(target); break;
            case "ListBoxPage": ListBoxes(target); break;
            case "ListViewPage": Lists(target); break;
            case "TreeViewPage": Trees(target); break;
            case "CalendarPage": Calendar(target); break;
            case "DatePickerPage": DatePicker(target); break;
            case "ExpanderPage": Expanders(target); break;
            case "GridPage": Grids(target); break;
            case "ResizeGripPage": WindowSizing(target); break;
            case "GridSplitterPage": Splitters(target); break;
            case "GroupBoxPage": Groups(target); break;
            case "StackPanelPage": StackPanels(target); break;
            case "BorderPage": Borders(target); break;
            case "MenuPage": Menus(target); break;
            case "TabControlPage": Tabs(target); break;
            case "FramePage": Frames(target); break;
            case "NavigationWindowPage": NavigationWindows(target); break;
            case "ProgressBarPage": Progress(target); break;
            case "ToolTipPage": ToolTips(target); break;
            case "LabelPage": Labels(target); break;
            case "TextBoxPage": TextBoxes(target); break;
            case "TextBlockPage": TextBlocks(target); break;
            case "RichTextEditPage": RichText(target); break;
            case "PasswordBoxPage": Passwords(target); break;
            case "HyperlinkPage": Hyperlinks(target); break;
            case "CanvasPage": Canvases(target); break;
            case "ImagePage": Images(target); break;
            case "FileAndFolderDialogsPage": FileDialogs(target); break;
            case "MessageBoxPage": Dialogs(target); break;
            case "ClipboardPage": ClipboardSamples(target); break;
            case "ColorsPage": Colors(target); break;
            case "TypographyPage": Typography(target); break;
            case "SpacingPage": Spacing(target); break;
            case "GeometryPage": Geometry(target); break;
            case "IconsPage": target.Children.Add(new Views.IconsView()); break;
            case "UserDashboardPage": target.Children.Add(new Views.UserDashboardView()); break;
            case "SettingsPage": Settings(target); break;
            default: throw new ArgumentOutOfRangeException(nameof(route), route, "No migrated sample exists for this route.");
        }
    }

    private static ExampleView Add(StackPanel target, string title, string xaml)
    {
        ExampleView example = ExampleView.FromXaml(title, xaml);
        target.Children.Add(example);
        return example;
    }

    private static void Add(StackPanel target, string title, FrameworkElement content, string source) =>
        target.Children.Add(new ExampleView(title, content, source));

    private static T Find<T>(ExampleView example, string name) where T : FrameworkElement =>
        example.Sample.FindName(name) is T element ? element :
            throw new InvalidOperationException($"Sample element '{name}' is missing or is not a {typeof(T).Name}.");

    private static T Identify<T>(T element, string id, string name) where T : FrameworkElement
    {
        AutomationProperties.SetAutomationId(element, id);
        AutomationProperties.SetName(element, name);
        return element;
    }

    private static TextBlock Status(string id)
    {
        var status = Identify(new TextBlock { TextWrapping = TextWrapping.Wrap }, id, "Sample result");
        AutomationProperties.SetLiveSetting(status, Microsoft.UI.Xaml.Automation.Peers.AutomationLiveSetting.Polite);
        return status;
    }

    private static void Note(StackPanel target, string text) =>
        target.Children.Add(new TextBlock { Text = text, TextWrapping = TextWrapping.Wrap });
}
