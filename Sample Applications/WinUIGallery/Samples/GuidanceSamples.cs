using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.Storage;

namespace WPFGallery.Samples;

internal static partial class SampleFactory
{
    private static void Colors(StackPanel target)
    {
        Note(target, "Use semantic ThemeResource brushes rather than fixed colors. The samples follow Light, Dark, and Windows Contrast themes.");
        foreach (var (title, resources) in new[]
        {
            ("Text", new[] { "TextFillColorPrimaryBrush", "TextFillColorSecondaryBrush", "TextFillColorDisabledBrush" }),
            ("Fill", new[] { "ControlFillColorDefaultBrush", "ControlFillColorSecondaryBrush", "ControlFillColorTertiaryBrush" }),
            ("Stroke", new[] { "ControlStrokeColorDefaultBrush", "ControlStrokeColorSecondaryBrush", "CardStrokeColorDefaultBrush" }),
            ("Background", new[] { "ApplicationPageBackgroundThemeBrush", "CardBackgroundFillColorDefaultBrush", "LayerFillColorDefaultBrush" }),
            ("Signal", new[] { "SystemFillColorSuccessBrush", "SystemFillColorCautionBrush", "SystemFillColorCriticalBrush" }),
            ("High Contrast", new[] { "SystemColorWindowColorBrush", "SystemColorWindowTextColorBrush", "SystemColorHighlightColorBrush" })
        })
        {
            string rows = string.Join("\n", resources.Select(resource => $$"""
                <StackPanel Orientation="Horizontal" Spacing="12">
                    <Border Width="48" Height="32" Background="{ThemeResource {{resource}}}"
                            BorderThickness="1" BorderBrush="{ThemeResource CardStrokeColorDefaultBrush}" />
                    <TextBlock Text="{{resource}}" TextWrapping="Wrap" VerticalAlignment="Center" />
                </StackPanel>
                """));
            Add(target, title, $"<StackPanel Spacing=\"12\">{rows}</StackPanel>");
        }
    }

    private static void Typography(StackPanel target)
    {
        foreach (string style in new[] { "Display", "TitleLarge", "Title", "Subtitle", "BodyLarge", "BodyStrong", "Body", "Caption" })
        {
            Add(target, $"{style} type style", $$"""
                <TextBlock Text="{{style}} - The quick brown fox" TextWrapping="Wrap" Style="{StaticResource {{style}}TextBlockStyle}" />
                """);
        }
    }

    private static void Spacing(StackPanel target)
    {
        Note(target, "Use a 4-pixel spacing rhythm. Standard controls keep their platform sizing and focus visuals.");
        foreach (int spacing in new[] { 4, 8, 12, 16, 24, 32 })
        {
            Add(target, $"{spacing}-pixel spacing", $"""
                <StackPanel Spacing="{spacing}">
                    <TextBlock Text="Spacing = {spacing}" />
                    <TextBox Header="Name" Text="Gallery" />
                    <Button Content="Continue" />
                </StackPanel>
                """);
        }
    }

    private static void Geometry(StackPanel target)
    {
        foreach (int radius in new[] { 0, 4, 8, 12 })
        {
            Add(target, $"{radius}-pixel corner radius", $$"""
                <Border CornerRadius="{{radius}}" BorderThickness="1"
                        BorderBrush="{ThemeResource AccentFillColorDefaultBrush}"
                        Background="{ThemeResource ControlFillColorSecondaryBrush}" Padding="24">
                    <TextBlock Text="CornerRadius = {{radius}}" />
                </Border>
                """);
        }
    }

    private static void Settings(StackPanel target)
    {
        var root = (FrameworkElement)App.Window.Content;
        var theme = Identify(new ComboBox
        {
            Header = "App theme",
            ItemsSource = new[] { "Use system setting", "Light", "Dark" },
            SelectedIndex = (int)root.RequestedTheme,
            HorizontalAlignment = HorizontalAlignment.Left,
            MinWidth = 220
        }, "AppTheme", "App theme");
        theme.SelectionChanged += (_, _) =>
        {
            root.RequestedTheme = (ElementTheme)theme.SelectedIndex;
            ApplicationData.Current.LocalSettings.Values["Theme"] = root.RequestedTheme.ToString();
        };
        target.Children.Add(theme);
        Note(target, "Gallery (WinUI 3) ports the WPF Gallery catalog to Microsoft.UI.Xaml. Original WPF sources remain available alongside this app.");
        target.Children.Add(new HyperlinkButton
        {
            Content = "Source and issues",
            NavigateUri = new Uri("https://github.com/microsoft/WPF-Samples")
        });
        target.Children.Add(new HyperlinkButton
        {
            Content = "WinUI documentation",
            NavigateUri = new Uri("https://learn.microsoft.com/windows/apps/winui/")
        });
        Note(target, "Dependencies: Microsoft.WindowsAppSDK, CommunityToolkit.Mvvm, CommunityToolkit.WinUI.Controls.Sizers.");
        Note(target, "This sample is provided under the repository's MIT license, without warranty.");
    }
}
