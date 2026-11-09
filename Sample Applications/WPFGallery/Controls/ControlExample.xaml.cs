// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and WPF UI Contributors.
// All Rights Reserved.

using System.IO;
using WPFGallery.Services;

namespace WPFGallery.Controls;

/// <summary>
/// A control that displays an example of a control
/// </summary>

[ContentProperty(Name = nameof(ExampleContent))]
public class ControlExample : Control
{
    public ControlExample()
    {
        DefaultStyleKey = typeof(ControlExample);
        CopyCommand = new AsyncRelayCommand<string>(CopyAsync);
    }

    public IAsyncRelayCommand<string> CopyCommand { get; }
    public static readonly DependencyProperty HeaderTextProperty = DependencyProperty.Register(
        nameof(HeaderText),
        typeof(string),
        typeof(ControlExample),
        new PropertyMetadata(null)
    );

    public static readonly DependencyProperty ExampleContentProperty = DependencyProperty.Register(
        nameof(ExampleContent),
        typeof(object),
        typeof(ControlExample),
        new PropertyMetadata(null)
    );

    public static readonly DependencyProperty XamlCodeProperty = DependencyProperty.Register(
        nameof(XamlCode),
        typeof(string),
        typeof(ControlExample),
        new PropertyMetadata(null)
    );

    public static readonly DependencyProperty XamlCodeSourceProperty = DependencyProperty.Register(
        nameof(XamlCodeSource),
        typeof(Uri),
        typeof(ControlExample),
        new PropertyMetadata(
            null,
            static (o, args) => ((ControlExample)o).OnXamlCodeSourceChanged((Uri)args.NewValue)
        )
    );

    public static readonly DependencyProperty CSharpCodeProperty = DependencyProperty.Register(
        nameof(CSharpCode),
        typeof(string),
        typeof(ControlExample),
        new PropertyMetadata(null)
    );

    public static readonly DependencyProperty CSharpCodeSourceProperty = DependencyProperty.Register(
        nameof(CSharpCodeSource),
        typeof(Uri),
        typeof(ControlExample),
        new PropertyMetadata(
            null,
            static (o, args) => ((ControlExample)o).OnCSharpCodeSourceChanged((Uri)args.NewValue)
        )
    );

    public string? HeaderText
    {
        get => (string)GetValue(HeaderTextProperty);
        set => SetValue(HeaderTextProperty, value);
    }

    public object? ExampleContent
    {
        get => GetValue(ExampleContentProperty);
        set => SetValue(ExampleContentProperty, value);
    }

    public string? XamlCode
    {
        get => (string)GetValue(XamlCodeProperty);
        set => SetValue(XamlCodeProperty, value);
    }

    public Uri? XamlCodeSource
    {
        get => (Uri)GetValue(XamlCodeSourceProperty);
        set => SetValue(XamlCodeSourceProperty, value);
    }

    public string? CSharpCode
    {
        get => (string)GetValue(CSharpCodeProperty);
        set => SetValue(CSharpCodeProperty, value);
    }

    public Uri? CSharpCodeSource
    {
        get => (Uri)GetValue(CSharpCodeSourceProperty);
        set => SetValue(CSharpCodeSourceProperty, value);
    }

    private void OnXamlCodeSourceChanged(Uri uri)
    {
        XamlCode = LoadResource(uri);
    }

    private void OnCSharpCodeSourceChanged(Uri uri)
    {
        CSharpCode = LoadResource(uri);
    }

    private async Task CopyAsync(string? language)
    {
        var code = language == "C#" ? CSharpCode : XamlCode;
        if (string.IsNullOrEmpty(code)) return;
        try
        {
            App.Services.GetRequiredService<IClipboardService>().CopyText(code);
            FrameworkElementAutomationPeer.FromElement(this)?.RaiseNotificationEvent(
                AutomationNotificationKind.Other, AutomationNotificationProcessing.ImportantMostRecent,
                "Source Code Copied", "SourceCodeCopied");
        }
        catch (Exception ex)
        {
            await App.Services.GetRequiredService<IDialogService>().ShowAsync(
                XamlRoot, "Error copying to clipboard: " + ex.Message, "Clipboard error", image: DialogImage.Error);
        }
    }

    private static string LoadResource(Uri uri)
    {
        var relativePath = Uri.UnescapeDataString(uri.AbsolutePath).TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
        return File.ReadAllText(Path.Combine(AppContext.BaseDirectory, relativePath), Encoding.UTF8);
    }
}

