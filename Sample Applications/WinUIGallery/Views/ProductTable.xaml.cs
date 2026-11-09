using Microsoft.UI.Xaml.Controls;
using WPFGallery.Models;

namespace WPFGallery.Views;

public sealed partial class ProductTable : UserControl
{
    public IReadOnlyList<Product> Products { get; } = Product.CreateSamples();
    public ProductTable() => InitializeComponent();
}
