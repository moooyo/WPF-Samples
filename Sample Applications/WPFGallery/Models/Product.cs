// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and WPF UI Contributors.
// All Rights Reserved.

namespace WPFGallery.Models;

using System.Globalization;

/// <summary>
/// Product class for DataGrid page
/// </summary>
public class Product : ObservableObject
{
    public int ProductId { get; set; }

    public int ProductCode { get; set; }

    public required string ProductName { get; set; }

    public required string QuantityPerUnit { get; set; }

    public double UnitPrice { get; set; }

    public int UnitsInStock { get; set; }

    public string AutomationId(string field) => $"{field}{ProductId}";

    private string _validationError = string.Empty;
    private readonly Dictionary<string, string> _validationErrors = [];
    public string ValidationError
    {
        get => _validationError;
        private set => SetProperty(ref _validationError, value);
    }

    public string ProductIdInput
    {
        get => ProductId.ToString(CultureInfo.CurrentCulture);
        set
        {
            if (TryReadInteger(value, "Product ID", out var number)) ProductId = number;
        }
    }

    public string ProductCodeInput
    {
        get => ProductCode.ToString(CultureInfo.CurrentCulture);
        set
        {
            if (TryReadInteger(value, "Product code", out var number)) ProductCode = number;
        }
    }

    public string UnitsInStockInput
    {
        get => UnitsInStock.ToString(CultureInfo.CurrentCulture);
        set
        {
            if (TryReadInteger(value, "Units in stock", out var number)) UnitsInStock = number;
        }
    }

    public double UnitPriceInput
    {
        get => UnitPrice;
        set
        {
            SetValidationError("Unit price", double.IsFinite(value) ? null : "Unit price must be a finite number.");
            if (double.IsFinite(value)) UnitPrice = value;
        }
    }

    private bool TryReadInteger(string value, string field, out int number)
    {
        bool valid = int.TryParse(value, NumberStyles.Integer, CultureInfo.CurrentCulture, out number);
        SetValidationError(field, valid ? null : $"{field} must be a whole number.");
        return valid;
    }

    private void SetValidationError(string field, string? error)
    {
        if (error is null) _validationErrors.Remove(field);
        else _validationErrors[field] = error;
        ValidationError = string.Join(" ", _validationErrors.Values);
    }
}
