// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and WPF UI Contributors.
// All Rights Reserved.

using WPFGallery.Models;

namespace WPFGallery.ViewModels;

public partial class DataGridPageViewModel : ObservableObject
{
    [ObservableProperty]
    public partial string PageTitle { get; set; } = "DataGrid";

    [ObservableProperty]
    public partial string PageDescription { get; set; } = "";

    [ObservableProperty]
    public partial ObservableCollection<Product> ProductsCollection { get; set; }

    public DataGridPageViewModel()
    {
        ProductsCollection = GenerateProducts();
    }

    private string? _sortColumn;
    private bool _descending;

    [RelayCommand]
    private void Sort(string column)
    {
        _descending = _sortColumn == column && !_descending;
        _sortColumn = column;
        Func<Product, IComparable> key = column switch
        {
            nameof(Product.ProductId) => product => product.ProductId,
            nameof(Product.ProductCode) => product => product.ProductCode,
            nameof(Product.ProductName) => product => product.ProductName,
            nameof(Product.QuantityPerUnit) => product => product.QuantityPerUnit,
            nameof(Product.UnitPrice) => product => product.UnitPrice,
            nameof(Product.UnitsInStock) => product => product.UnitsInStock,
            _ => throw new ArgumentOutOfRangeException(nameof(column))
        };
        var sorted = (_descending
            ? ProductsCollection.OrderByDescending(key) : ProductsCollection.OrderBy(key)).ToArray();
        ProductsCollection.Clear();
        foreach (var product in sorted) ProductsCollection.Add(product);
    }

    private ObservableCollection<Product> GenerateProducts()
    {
        var random = new Random();
        var products = new ObservableCollection<Product> { };

        var adjectives = new[] { "Red", "Blueberry" };
        var names = new[] { "Marmalade", "Dumplings", "Soup" };
        var quantitiesPerUnit = new[] { "6", "10", "12", "16", "24", "48" };
        //var units = new[] { "grams", "kilograms", "milliliters" };

        for (int i = 0; i < 50; i++)
        {
            products.Add(
                new Product
                {
                    ProductId = i,
                    ProductCode = i,
                    ProductName =
                        adjectives[random.Next(0, adjectives.Length)]
                        + " "
                        + names[random.Next(0, names.Length)],
                    QuantityPerUnit = quantitiesPerUnit[random.Next(0, quantitiesPerUnit.Length)],
                    UnitPrice = Math.Round(random.NextDouble() * 20.0, 3),
                    UnitsInStock = random.Next(0, 100)
                }
            );
        }

        return products;
    }
}