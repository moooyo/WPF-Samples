using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace WPFGallery.Samples;

internal static partial class SampleFactory
{
    private static void Buttons(StackPanel target)
    {
        var example = Add(target, "Standard and accent buttons", """
            <StackPanel Spacing="12">
                <Button x:Name="Standard" Content="Standard WinUI button" AutomationProperties.AutomationId="DemoButton" />
                <Button x:Name="Accent" Content="Accent button" Style="{StaticResource AccentButtonStyle}" />
                <CheckBox x:Name="Disabled" Content="Disable buttons" />
                <TextBlock x:Name="Result" AutomationProperties.AutomationId="ButtonResult" AutomationProperties.LiveSetting="Polite" />
            </StackPanel>
            """);
        Button standard = Find<Button>(example, "Standard");
        Button accent = Find<Button>(example, "Accent");
        TextBlock result = Find<TextBlock>(example, "Result");
        int clicks = 0;
        standard.Click += (_, _) => result.Text = $"Clicked {++clicks} time(s).";
        accent.Click += (_, _) => result.Text = $"Clicked {++clicks} time(s).";
        CheckBox disabled = Find<CheckBox>(example, "Disabled");
        disabled.Checked += (_, _) => standard.IsEnabled = accent.IsEnabled = false;
        disabled.Unchecked += (_, _) => standard.IsEnabled = accent.IsEnabled = true;
    }

    private static void CheckBoxes(StackPanel target)
    {
        Add(target, "Two-state and three-state CheckBox", """
            <StackPanel Spacing="8">
                <CheckBox Content="Two-state CheckBox" />
                <CheckBox Content="Three-state CheckBox" IsThreeState="True" IsChecked="{x:Null}" />
            </StackPanel>
            """);
        var example = Add(target, "Select all", """
            <StackPanel>
                <CheckBox x:Name="All" Content="Select all" IsThreeState="True" AutomationProperties.AutomationId="SelectAll" />
                <CheckBox x:Name="One" Content="Option 1" Margin="24,0,0,0" AutomationProperties.AutomationId="OptionOne" />
                <CheckBox x:Name="Two" Content="Option 2" Margin="24,0,0,0" />
                <CheckBox x:Name="Three" Content="Option 3" Margin="24,0,0,0" />
            </StackPanel>
            """);
        CheckBox all = Find<CheckBox>(example, "All");
        CheckBox[] options = [Find<CheckBox>(example, "One"), Find<CheckBox>(example, "Two"), Find<CheckBox>(example, "Three")];
        bool updating = false;
        void SetOptions(object sender, RoutedEventArgs args)
        {
            if (updating) return;
            updating = true;
            foreach (CheckBox option in options) option.IsChecked = all.IsChecked == true;
            all.IsChecked = options.All(option => option.IsChecked == true);
            updating = false;
        }
        void SetAll(object sender, RoutedEventArgs args)
        {
            if (updating) return;
            updating = true;
            all.IsChecked = options.All(option => option.IsChecked == true) ? true
                : options.All(option => option.IsChecked == false) ? false : null;
            updating = false;
        }
        all.Checked += SetOptions;
        all.Unchecked += SetOptions;
        all.Indeterminate += SetOptions;
        foreach (CheckBox option in options)
        {
            option.Checked += SetAll;
            option.Unchecked += SetAll;
        }
    }

    private static void ComboBoxes(StackPanel target)
    {
        Add(target, "Inline items", """
            <ComboBox Header="Color" Width="220" HorizontalAlignment="Left" SelectedIndex="0">
                <x:String>Blue</x:String><x:String>Green</x:String><x:String>Red</x:String><x:String>Yellow</x:String>
            </ComboBox>
            """);
        var fonts = new ComboBox
        {
            Header = "Font family",
            Width = 220,
            HorizontalAlignment = HorizontalAlignment.Left,
            ItemsSource = new[] { "Segoe UI", "Arial", "Cascadia Code", "Consolas" },
            SelectedIndex = 0
        };
        Add(target, "ItemsSource", fonts, """
            <ComboBox Header="Font family" ItemsSource="{x:Bind FontFamilies}" SelectedIndex="0" />
            """);
        Add(target, "Editable ComboBox", """
            <ComboBox Header="Font size" Width="220" HorizontalAlignment="Left" IsEditable="True" SelectedIndex="1">
                <x:String>12</x:String><x:String>14</x:String><x:String>18</x:String><x:String>24</x:String>
            </ComboBox>
            """);
    }

    private static void RadioButtons(StackPanel target)
    {
        Add(target, "Mutually exclusive options", """
            <RadioButtons Header="Choose an option" SelectedIndex="0">
                <x:String>Option 1</x:String><x:String>Option 2</x:String><x:String>Option 3</x:String>
            </RadioButtons>
            """);
        Add(target, "Right-to-left RadioButton group", """
            <StackPanel HorizontalAlignment="Left" FlowDirection="RightToLeft">
                <RadioButton Content="Option 1" GroupName="RtlOptions" IsChecked="True" />
                <RadioButton Content="Option 2" GroupName="RtlOptions" />
                <RadioButton Content="Option 3" GroupName="RtlOptions" />
            </StackPanel>
            """);
    }

    private static void Sliders(StackPanel target)
    {
        Add(target, "Simple slider", """
            <Slider Header="Value" Width="260" HorizontalAlignment="Left" Minimum="0" Maximum="100" Value="25" />
            """);
        Add(target, "Stepped range", """
            <Slider Header="Range from 500 to 1000" Width="260" HorizontalAlignment="Left"
                    Minimum="500" Maximum="1000" StepFrequency="50" TickFrequency="50" SnapsTo="StepValues" Value="500" />
            """);
        Add(target, "Tick marks", """
            <Slider Header="Value with tick marks" Width="260" HorizontalAlignment="Left"
                    Minimum="0" Maximum="100" TickFrequency="20" TickPlacement="Outside" StepFrequency="20" />
            """);
        Add(target, "Vertical slider", """
            <Slider Height="180" HorizontalAlignment="Left" Orientation="Vertical"
                    Minimum="0" Maximum="100" TickFrequency="20" TickPlacement="Outside"
                    AutomationProperties.Name="Vertical value" />
            """);
    }

    private static void Calendar(StackPanel target) =>
        Add(target, "CalendarView replaces WPF Calendar", """
            <CalendarView SelectionMode="Single" HorizontalAlignment="Left" AutomationProperties.Name="Choose a date" />
            """);

    private static void DatePicker(StackPanel target) =>
        Add(target, "CalendarDatePicker and DatePicker", """
            <StackPanel Spacing="16">
                <CalendarDatePicker Header="Calendar date" PlaceholderText="Choose a date" />
                <DatePicker Header="Date with individual fields" />
            </StackPanel>
            """);

    private static void Progress(StackPanel target)
    {
        Add(target, "Determinate progress", """
            <StackPanel Spacing="8">
                <ProgressBar Value="60" Maximum="100" AutomationProperties.Name="Download progress" />
                <TextBlock Text="60% complete" />
            </StackPanel>
            """);
        Add(target, "Indeterminate progress", """
            <ProgressBar IsIndeterminate="True" AutomationProperties.Name="Loading" />
            """);
    }

    private static void ToolTips(StackPanel target) =>
        Add(target, "A button with a ToolTip", """
            <Button Content="Hover or focus me" HorizontalAlignment="Left" ToolTipService.ToolTip="This is a helpful tooltip." />
            """);
}
