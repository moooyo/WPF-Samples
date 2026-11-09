using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using WPFGallery.Controls;

namespace WPFGallery.Samples;

internal static partial class SampleFactory
{
    private static void Products(StackPanel target)
    {
        Note(target, "WinUI 3 has no built-in DataGrid. A ListView with Grid-based column headers and rows provides the table.");
        var table = new Views.ProductTable();
        Add(target, "Products table", table, """
            <Grid RowDefinitions="Auto,*">
                <Grid ColumnDefinitions="64,*,100,100,100">
                    <!-- Accessible column headers: ID, Product, Quantity, Price, Stock -->
                </Grid>
                <ListView Grid.Row="1" ItemsSource="{x:Bind Products}">
                    <!-- Typed DataTemplate containing the same Grid columns -->
                </ListView>
            </Grid>
            """);
    }

    private static void ListBoxes(StackPanel target)
    {
        Add(target, "Inline ListBox", """
            <ListBox Height="160" SelectedIndex="0" AutomationProperties.Name="Colors">
                <x:String>Blue</x:String><x:String>Green</x:String><x:String>Red</x:String><x:String>Yellow</x:String>
            </ListBox>
            """);
        var list = new ListBox
        {
            Height = 160,
            ItemsSource = new[] { "Aster", "Dahlia", "Iris", "Orchid", "Tulip" },
            SelectedIndex = 2
        };
        Identify(list, "FlowerList", "Flowers");
        Add(target, "ListBox with ItemsSource", list, """
            <ListBox Height="160" ItemsSource="{x:Bind Flowers}" SelectedIndex="2" />
            """);
    }

    private static void Lists(StackPanel target)
    {
        var example = Add(target, "ListView with selection support", """
            <StackPanel Spacing="12">
                <ComboBox x:Name="Mode" Header="Selection mode" SelectedIndex="0">
                    <x:String>Single</x:String><x:String>Multiple</x:String><x:String>Extended</x:String>
                </ComboBox>
                <ListView x:Name="People" Height="200" AutomationProperties.Name="People" AutomationProperties.AutomationId="DemoPeople">
                    <x:String>John Doe - Luminary Nexus</x:String>
                    <x:String>Adrianna Cisneros - Horizon Ventures</x:String>
                    <x:String>Lucas Marsh - CrestWave Dynamics</x:String>
                    <x:String>Phoebe Munoz - StellarEdge Ventures</x:String>
                </ListView>
                <TextBlock x:Name="Selection" AutomationProperties.LiveSetting="Polite" AutomationProperties.AutomationId="ListSelectionResult" />
            </StackPanel>
            """);
        ListView list = Find<ListView>(example, "People");
        TextBlock selection = Find<TextBlock>(example, "Selection");
        Find<ComboBox>(example, "Mode").SelectionChanged += (sender, _) =>
            list.SelectionMode = (ListViewSelectionMode)(((ComboBox)sender).SelectedIndex + 1);
        list.SelectionChanged += (_, _) => selection.Text = $"{list.SelectedItems.Count} selected";
        Products(target);
    }

    private static void Trees(StackPanel target) =>
        Add(target, "Hierarchical TreeView", """
            <TreeView Height="300" AutomationProperties.Name="Documents">
                <TreeView.RootNodes>
                    <TreeViewNode Content="Work Documents" IsExpanded="True">
                        <TreeViewNode.Children>
                            <TreeViewNode Content="Feature Schedule" />
                            <TreeViewNode Content="Overall Project Plan" />
                        </TreeViewNode.Children>
                    </TreeViewNode>
                    <TreeViewNode Content="Personal Documents" IsExpanded="True">
                        <TreeViewNode.Children>
                            <TreeViewNode Content="Contractor contact info" />
                            <TreeViewNode Content="Home Remodel">
                                <TreeViewNode.Children>
                                    <TreeViewNode Content="Paint Color Scheme" />
                                    <TreeViewNode Content="Flooring Woodgrain Type" />
                                    <TreeViewNode Content="Kitchen Cabinet Style" />
                                </TreeViewNode.Children>
                            </TreeViewNode>
                        </TreeViewNode.Children>
                    </TreeViewNode>
                </TreeView.RootNodes>
            </TreeView>
            """);
}
