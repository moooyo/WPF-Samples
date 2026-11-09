using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Automation;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Markup;
using Windows.ApplicationModel.DataTransfer;

namespace WPFGallery.Controls;

public sealed partial class ExampleView : UserControl
{
    public FrameworkElement Sample { get; }

    public ExampleView(string title, FrameworkElement sample, string source)
    {
        InitializeComponent();
        Sample = sample;
        Heading.Text = title;
        SamplePresenter.Content = sample;
        SourceText.Text = source;
        AutomationProperties.SetName(CopySource, $"Copy source for {title}");
        AutomationProperties.SetAutomationId(CopySource, $"CopySource_{title}");
        AutomationProperties.SetAutomationId(SourceExpander, $"Source_{title}");
    }

    public static ExampleView FromXaml(string title, string source)
    {
        string markup = $"""
            <Grid xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
                  xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
            {source}
            </Grid>
            """;
        return new ExampleView(title, (FrameworkElement)XamlReader.Load(markup), source);
    }

    private void CopySource_Click(object sender, RoutedEventArgs args)
    {
        try
        {
            var data = new DataPackage();
            data.SetText(SourceText.Text);
            Clipboard.SetContent(data);
            CopyStatus.Text = "Source copied.";
        }
        catch (System.Runtime.InteropServices.COMException exception)
        {
            CopyStatus.Text = $"Cannot copy source: {exception.Message}";
        }
    }
}
