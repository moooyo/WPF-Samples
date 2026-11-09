namespace WPFGallery.Models;

public sealed class Product
{
    public int Id { get; init; }
    public string Name { get; init; } = "";
    public string Quantity { get; init; } = "";
    public string Price { get; init; } = "";
    public int Stock { get; init; }

    public static IReadOnlyList<Product> CreateSamples() => Enumerable.Range(1, 50).Select(index => new Product
    {
        Id = index,
        Name = $"{(index % 2 == 0 ? "Red" : "Blueberry")} {new[] { "Marmalade", "Dumplings", "Soup" }[index % 3]}",
        Quantity = new[] { "6", "10", "12", "16", "24", "48" }[index % 6],
        Price = (index * 0.39m).ToString("C2", System.Globalization.CultureInfo.CurrentCulture),
        Stock = index * 7 % 100
    }).ToArray();
}
