using CommunityToolkit.WinUI.Controls;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.Graphics;
using WPFGallery.Views;

namespace WPFGallery.Samples;

internal static partial class SampleFactory
{
    private static void Expanders(StackPanel target) =>
        Add(target, "An Expander with header and content", """
            <Expander Header="More information" HorizontalAlignment="Stretch">
                <TextBlock Text="This content is shown when the Expander is expanded." TextWrapping="Wrap" />
            </Expander>
            """);

    private static void Grids(StackPanel target)
    {
        Add(target, "A 3 x 3 Grid", """
            <Grid Height="180" RowSpacing="4" ColumnSpacing="4">
                <Grid.RowDefinitions>
                    <RowDefinition /><RowDefinition /><RowDefinition />
                </Grid.RowDefinitions>
                <Grid.ColumnDefinitions>
                    <ColumnDefinition /><ColumnDefinition /><ColumnDefinition />
                </Grid.ColumnDefinitions>
                <Border Background="{ThemeResource AccentFillColorDefaultBrush}"><TextBlock Text="0,0" Margin="8" /></Border>
                <Border Grid.Column="1" Background="{ThemeResource ControlFillColorSecondaryBrush}"><TextBlock Text="0,1" Margin="8" /></Border>
                <Border Grid.Column="2" Background="{ThemeResource ControlFillColorSecondaryBrush}"><TextBlock Text="0,2" Margin="8" /></Border>
                <TextBlock Grid.Row="1" Text="1,0" Margin="8" />
                <TextBlock Grid.Row="1" Grid.Column="1" Text="1,1" Margin="8" />
                <TextBlock Grid.Row="1" Grid.Column="2" Text="1,2" Margin="8" />
                <TextBlock Grid.Row="2" Text="2,0" Margin="8" />
                <TextBlock Grid.Row="2" Grid.Column="1" Text="2,1" Margin="8" />
                <TextBlock Grid.Row="2" Grid.Column="2" Text="2,2" Margin="8" />
            </Grid>
            """);
        Add(target, "Custom sizing and spanning", """
            <Grid Height="180" RowSpacing="8" ColumnSpacing="8">
                <Grid.RowDefinitions><RowDefinition Height="Auto" /><RowDefinition /></Grid.RowDefinitions>
                <Grid.ColumnDefinitions><ColumnDefinition Width="120" /><ColumnDefinition /></Grid.ColumnDefinitions>
                <TextBlock Grid.ColumnSpan="2" Text="Header spans both columns" FontWeight="SemiBold" />
                <Border Grid.Row="1" Background="{ThemeResource ControlFillColorSecondaryBrush}" Padding="12">
                    <TextBlock Text="120 px" />
                </Border>
                <Border Grid.Row="1" Grid.Column="1" Background="{ThemeResource ControlFillColorSecondaryBrush}" Padding="12">
                    <TextBlock Text="Remaining space" />
                </Border>
            </Grid>
            """);
        Note(target, "WPF's Grid shorthand syntax is replaced with explicit RowDefinitions and ColumnDefinitions.");
    }

    private static void Splitters(StackPanel target)
    {
        var grid = new Grid { Height = 220 };
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star), MinWidth = 100 });
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star), MinWidth = 100 });
        var left = new TextBox { Header = "Left pane", Text = "Drag the divider or use its arrow keys.", AcceptsReturn = true };
        var right = new TextBox { Header = "Right pane", Text = "Both columns resize.", AcceptsReturn = true };
        var splitter = Identify(new GridSplitter
        {
            Width = 12,
            ResizeDirection = GridSplitter.GridResizeDirection.Columns,
            ResizeBehavior = GridSplitter.GridResizeBehavior.PreviousAndNext,
            HorizontalAlignment = HorizontalAlignment.Stretch
        }, "DemoGridSplitter", "Resize columns");
        Grid.SetColumn(splitter, 1);
        Grid.SetColumn(right, 2);
        grid.Children.Add(left);
        grid.Children.Add(splitter);
        grid.Children.Add(right);
        Add(target, "CommunityToolkit GridSplitter", grid, """
            <Grid ColumnDefinitions="*,Auto,*">
                <TextBox Header="Left pane" />
                <controls:GridSplitter Grid.Column="1" Width="12"
                    ResizeDirection="Columns" ResizeBehavior="PreviousAndNext" />
                <TextBox Grid.Column="2" Header="Right pane" />
            </Grid>
            """);
    }

    private static void Groups(StackPanel target)
    {
        Note(target, "WinUI 3 replaces GroupBox with a labelled container and standard controls.");
        Add(target, "Accessible group", """
            <Border BorderBrush="{ThemeResource CardStrokeColorDefaultBrush}" BorderThickness="1" Padding="16" CornerRadius="8">
                <StackPanel Spacing="8" AutomationProperties.Name="Account options">
                    <TextBlock Text="Account options" FontWeight="SemiBold" AutomationProperties.HeadingLevel="Level3" />
                    <TextBox Header="Display name" Text="Gallery user" />
                    <CheckBox Content="Receive updates" />
                </StackPanel>
            </Border>
            """);
    }

    private static void WindowSizing(StackPanel target)
    {
        Note(target, "WinUI windows use the native resize border instead of WPF ResizeGrip. AppWindow also supports explicit sizing.");
        var example = Add(target, "Resize the main window", """
            <StackPanel Spacing="12">
                <NumberBox x:Name="WidthInput" Header="Width (DIPs)" Minimum="780" Maximum="1920" Value="1200" />
                <NumberBox x:Name="HeightInput" Header="Height (DIPs)" Minimum="470" Maximum="1200" Value="800" />
                <Button x:Name="Resize" Content="Resize window" AutomationProperties.AutomationId="ResizeWindow" />
                <TextBlock x:Name="ResizeStatus" AutomationProperties.LiveSetting="Polite" AutomationProperties.AutomationId="ResizeResult" />
            </StackPanel>
            """);
        NumberBox widthInput = Find<NumberBox>(example, "WidthInput");
        NumberBox heightInput = Find<NumberBox>(example, "HeightInput");
        TextBlock resizeStatus = Find<TextBlock>(example, "ResizeStatus");
        Find<Button>(example, "Resize").Click += (_, _) =>
        {
            double width = widthInput.Value;
            double height = heightInput.Value;
            if (double.IsFinite(width) && double.IsFinite(height))
            {
                double scale = App.Window.Content.XamlRoot.RasterizationScale;
                App.Window.AppWindow.Resize(new SizeInt32((int)(width * scale), (int)(height * scale)));
                resizeStatus.Text = $"Window resized to {width} x {height} DIPs.";
            }
            else
            {
                resizeStatus.Text = "Enter a valid width and height.";
            }
        };
    }

    private static void StackPanels(StackPanel target)
    {
        Add(target, "Vertical StackPanel", """
            <StackPanel Spacing="8">
                <Button Content="First" /><Button Content="Second" /><Button Content="Third" />
            </StackPanel>
            """);
        Add(target, "Horizontal StackPanel", """
            <StackPanel Orientation="Horizontal" Spacing="8">
                <Button Content="First" /><Button Content="Second" /><Button Content="Third" />
            </StackPanel>
            """);
    }

    private static void Borders(StackPanel target)
    {
        Add(target, "Basic Border", """
            <Border BorderBrush="{ThemeResource AccentFillColorDefaultBrush}" BorderThickness="2" Padding="16">
                <TextBlock Text="Content inside a Border" />
            </Border>
            """);
        Add(target, "Rounded corners", """
            <Border BorderBrush="{ThemeResource AccentFillColorDefaultBrush}" BorderThickness="2"
                    CornerRadius="8" Padding="16" Background="{ThemeResource ControlFillColorSecondaryBrush}">
                <TextBlock Text="Rounded Border" />
            </Border>
            """);
        Add(target, "Different thickness on each side", """
            <Border BorderBrush="{ThemeResource AccentFillColorDefaultBrush}" BorderThickness="1,2,4,8" Padding="16">
                <TextBlock Text="Different border thickness" />
            </Border>
            """);
    }

    private static void Menus(StackPanel target)
    {
        var panel = new StackPanel { Spacing = 12 };
        var result = Status("MenuResult");
        var menu = new MenuBar();
        foreach (var (header, labels) in new[]
        {
            ("File", new[] { "New", "Open", "Save" }),
            ("Edit", new[] { "Cut", "Copy", "Paste" })
        })
        {
            var item = new MenuBarItem { Title = header };
            foreach (string label in labels)
            {
                var command = new MenuFlyoutItem { Text = label };
                command.Click += (_, _) => result.Text = $"{label} selected.";
                item.Items.Add(command);
            }
            menu.Items.Add(item);
        }
        panel.Children.Add(menu);
        panel.Children.Add(result);
        Add(target, "MenuBar and MenuFlyoutItem replace WPF Menu", panel, """
            <MenuBar>
                <MenuBarItem Title="File">
                    <MenuFlyoutItem Text="New" />
                    <MenuFlyoutItem Text="Open" />
                    <MenuFlyoutItem Text="Save" />
                </MenuBarItem>
            </MenuBar>
            """);
    }

    private static void Tabs(StackPanel target)
    {
        var tabs = Identify(new TabView { Height = 240, IsAddTabButtonVisible = true }, "DemoTabs", "Documents");
        int nextId = 0;
        void AddTab()
        {
            var tab = new TabViewItem
            {
                Header = $"Document {++nextId}",
                IsClosable = true,
                Content = new TextBox { Header = "Document text", Text = $"Content for document {nextId}", AcceptsReturn = true }
            };
            tabs.TabItems.Add(tab);
            tabs.SelectedItem = tab;
        }
        AddTab();
        AddTab();
        tabs.AddTabButtonClick += (_, _) => AddTab();
        tabs.TabCloseRequested += (_, args) => tabs.TabItems.Remove(args.Tab);
        Add(target, "TabView replaces TabControl", tabs, """
            <TabView IsAddTabButtonVisible="True" AddTabButtonClick="AddTab" TabCloseRequested="CloseTab">
                <TabViewItem Header="Document 1" IsClosable="True">
                    <TextBox Header="Document text" AcceptsReturn="True" />
                </TabViewItem>
            </TabView>
            """);
    }

    private static FrameworkElement NavigationSurface()
    {
        var panel = new Grid { Height = 240, RowSpacing = 12 };
        panel.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        panel.RowDefinitions.Add(new RowDefinition());
        var frame = Identify(new Frame(), "DemoFrame", "Sample navigation frame");
        var buttons = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 8 };
        var back = new Button { Content = "Back", IsEnabled = false };
        var forward = new Button { Content = "Forward", IsEnabled = false };
        var next = Identify(new Button { Content = "Next page" }, "FrameNext", "Next page");
        buttons.Children.Add(back);
        buttons.Children.Add(forward);
        buttons.Children.Add(next);
        panel.Children.Add(buttons);
        Grid.SetRow(frame, 1);
        panel.Children.Add(frame);
        frame.Navigated += (_, _) =>
        {
            back.IsEnabled = frame.CanGoBack;
            forward.IsEnabled = frame.CanGoForward;
        };
        back.Click += (_, _) => { if (frame.CanGoBack) frame.GoBack(); };
        forward.Click += (_, _) => { if (frame.CanGoForward) frame.GoForward(); };
        next.Click += (_, _) => frame.Navigate(typeof(SampleNavigationPage),
            frame.Content is SampleNavigationPage { Number: 1 } ? 2 : 1);
        frame.Navigate(typeof(SampleNavigationPage), 1);
        return panel;
    }

    private static void Frames(StackPanel target) =>
        Add(target, "Frame navigation with back and forward history", NavigationSurface(), """
            frame.Navigate(typeof(SampleNavigationPage), pageNumber);
            if (frame.CanGoBack) frame.GoBack();
            if (frame.CanGoForward) frame.GoForward();
            """);

    private static void NavigationWindows(StackPanel target)
    {
        Note(target, "NavigationWindow is replaced by a WinUI Window hosting a Frame.");
        var open = Identify(new Button { Content = "Open navigation window" }, "OpenNavigationWindow", "Open navigation window");
        open.Click += (_, _) =>
        {
            var window = new Window
            {
                Title = "Gallery navigation sample",
                Content = new Border { Padding = new Thickness(24), Child = NavigationSurface() }
            };
            App.KeepWindow(window);
            window.Activate();
        };
        Add(target, "A secondary navigation window", open, """
            var window = new Window { Content = new Frame() };
            window.Activate();
            """);
    }
}
