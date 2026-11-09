using Microsoft.UI.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace WPFGallery.Samples;

internal static partial class SampleFactory
{
    private static void Labels(StackPanel target)
    {
        Note(target, "WinUI uses control headers and AutomationProperties.LabeledBy instead of WPF Label.Target.");
        Add(target, "A labelled TextBox", """
            <TextBox Header="Display name" PlaceholderText="Enter your name" />
            """);
        Add(target, "An explicit accessible label", """
            <StackPanel Spacing="8">
                <TextBlock x:Name="Label" Text="Email address" />
                <TextBox AutomationProperties.LabeledBy="{Binding ElementName=Label}" />
            </StackPanel>
            """);
    }

    private static void TextBoxes(StackPanel target)
    {
        Add(target, "A simple TextBox", """
            <TextBox Header="Text" PlaceholderText="Type something" AutomationProperties.AutomationId="DemoTextBox" />
            """);
        var example = Add(target, "Alphabetic input validation", """
            <StackPanel Spacing="8">
                <TextBox x:Name="Input" Header="Letters only" AutomationProperties.AutomationId="ValidatedText" />
                <InfoBar x:Name="Error" Severity="Error" Title="Invalid input"
                         Message="Use letters only." IsOpen="False" IsClosable="False"
                         AutomationProperties.AutomationId="ValidationError" />
            </StackPanel>
            """);
        TextBox input = Find<TextBox>(example, "Input");
        InfoBar error = Find<InfoBar>(example, "Error");
        input.TextChanged += (_, _) => error.IsOpen = input.Text.Any(character => !char.IsLetter(character));
        Add(target, "Multi-line TextBox", """
            <TextBox Header="Notes" Height="140" AcceptsReturn="True" TextWrapping="Wrap"
                     Text="A multi-line editor.&#10;Press Enter to add another line." />
            """);
    }

    private static void TextBlocks(StackPanel target)
    {
        Add(target, "Text and styles", """
            <StackPanel Spacing="12">
                <TextBlock Text="A simple TextBlock." />
                <TextBlock Text="A styled TextBlock." Style="{StaticResource SubtitleTextBlockStyle}" />
                <TextBlock><Run Text="Inline text: " /><Bold>bold</Bold><Run Text=", " /><Italic>italic</Italic><Run Text=", " /><Underline>underlined</Underline></TextBlock>
                <TextBlock TextWrapping="Wrap" Text="TextWrapping lets long paragraphs adapt to the available space. Resize the window to see the text reflow." />
            </StackPanel>
            """);
    }

    private static void RichText(StackPanel target)
    {
        var editor = Identify(new RichEditBox { Height = 200 }, "RichTextEditor", "Rich text editor");
        editor.Document.SetText(TextSetOptions.None, "Edit and format this rich text document.");
        var panel = new StackPanel { Spacing = 12 };
        var toolbar = new CommandBar { DefaultLabelPosition = CommandBarDefaultLabelPosition.Right };
        var bold = new AppBarButton { Label = "Bold", Icon = new SymbolIcon(Symbol.Bold) };
        var italic = new AppBarButton { Label = "Italic", Icon = new SymbolIcon(Symbol.Italic) };
        bold.Click += (_, _) =>
        {
            editor.Document.Selection.CharacterFormat.Bold = FormatEffect.Toggle;
            editor.Focus(FocusState.Programmatic);
        };
        italic.Click += (_, _) =>
        {
            editor.Document.Selection.CharacterFormat.Italic = FormatEffect.Toggle;
            editor.Focus(FocusState.Programmatic);
        };
        toolbar.PrimaryCommands.Add(bold);
        toolbar.PrimaryCommands.Add(italic);
        panel.Children.Add(toolbar);
        panel.Children.Add(editor);
        Add(target, "RichEditBox replaces RichTextBox", panel, """
            <RichEditBox Height="200" AutomationProperties.Name="Rich text editor" />
            // Format the selection:
            editor.Document.Selection.CharacterFormat.Bold = FormatEffect.Toggle;
            """);
    }

    private static void Passwords(StackPanel target) =>
        Add(target, "PasswordBox with reveal support", """
            <PasswordBox Header="Password" PasswordRevealMode="Peek" AutomationProperties.AutomationId="DemoPassword" />
            """);

    private static void Hyperlinks(StackPanel target) =>
        Add(target, "HyperlinkButton and inline Hyperlink", """
            <StackPanel Spacing="12">
                <HyperlinkButton Content="WinUI documentation" NavigateUri="https://learn.microsoft.com/windows/apps/winui/" />
                <TextBlock TextWrapping="Wrap">
                    <Run Text="Read the " />
                    <Hyperlink NavigateUri="https://github.com/microsoft/WPF-Samples">original Gallery source</Hyperlink>
                    <Run Text=" on GitHub." />
                </TextBlock>
            </StackPanel>
            """);

    private static void Canvases(StackPanel target) =>
        Add(target, "Canvas inside a Viewbox", """
            <Viewbox Height="240" HorizontalAlignment="Left">
                <Canvas Width="400" Height="240" Background="{ThemeResource ControlFillColorSecondaryBrush}">
                    <Rectangle Canvas.Left="24" Canvas.Top="24" Width="160" Height="100"
                               Fill="{ThemeResource AccentFillColorDefaultBrush}" />
                    <Ellipse Canvas.Left="220" Canvas.Top="80" Width="120" Height="120"
                             Stroke="{ThemeResource TextFillColorPrimaryBrush}" StrokeThickness="4" />
                    <TextBlock Canvas.Left="24" Canvas.Top="160" Text="Canvas coordinates" />
                </Canvas>
            </Viewbox>
            """);

    private static void Images(StackPanel target) =>
        Add(target, "Local image with ms-appx URI", """
            <Image Source="ms-appx:///Assets/Gallery/win11-dashboard.png" Height="280"
                   Stretch="Uniform" HorizontalAlignment="Left" AutomationProperties.Name="Dashboard preview" />
            """);
}
