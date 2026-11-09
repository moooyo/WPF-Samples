using WPFGallery.Services;
// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and WPF UI Contributors.
// All Rights Reserved.

namespace WPFGallery.ViewModels;

/// <summary>
/// Interaction logic for MessageBoxPage.xaml
/// </summary>
public partial class MessageBoxPageViewModel : ObservableObject
{
    [ObservableProperty]
    public partial string PageTitle { get; set; } = "MessageBox";

    [ObservableProperty]
    public partial string PageDescription { get; set; } = "";

    [ObservableProperty]
    public partial string DefaultMessageResult { get; set; } = "No message shown yet";

    [ObservableProperty]
    public partial string CustomTitleResult { get; set; } = "No message shown yet";

    [ObservableProperty]
    public partial int SelectedButtonIndex { get; set; } = 0;

    [ObservableProperty]
    public partial string DifferentButtonsResult { get; set; } = "No button clicked yet";

    [ObservableProperty]
    public partial string DifferentButtonsXamlCode { get; set; } = "<Button Content=\"Show MessageBox\" Click=\"ShowDialogButtons_Click\" />";

    [ObservableProperty]
    public partial string DifferentButtonsCSharpCode { get; set; } = string.Format(_differentButtonsMessageBoxSampleCSharpCodeString, "\tawait dialogService.ShowAsync(XamlRoot, \"Message\", \"Title\", DialogButtons.OK);");

    partial void OnSelectedButtonIndexChanged(int value)
    {
        UpdateButtonCodeSnippets(value);
    }

    private void UpdateButtonCodeSnippets(int index)
    {
        string content = (DialogButtons)index switch
        {
            DialogButtons.OK => "\tawait dialogService.ShowAsync(XamlRoot, \"Message\", \"Title\", DialogButtons.OK);",
            DialogButtons.OKCancel => "\tvar result = await dialogService.ShowAsync(XamlRoot, \"Message\", \"Title\", DialogButtons.OKCancel);\n\tif (result == DialogResult.OK)\n\t{\n\t    // User clicked OK\n\t}",
            DialogButtons.AbortRetryIgnore => "\tvar result = await dialogService.ShowAsync(XamlRoot, \"Message\", \"Title\", DialogButtons.AbortRetryIgnore);\n\tif (result == DialogResult.Abort)\n\t{\n\t    // User clicked Abort\n\t}\n\telse if (result == DialogResult.Retry)\n\t{\n\t    // User clicked Retry\n\t}\n\telse if (result == DialogResult.Ignore)\n\t{\n\t    // User clicked Ignore\n\t}",
            DialogButtons.YesNoCancel => "\tvar result = await dialogService.ShowAsync(XamlRoot, \"Message\", \"Title\", DialogButtons.YesNoCancel);\n\tif (result == DialogResult.Yes)\n\t{\n\t    // User clicked Yes\n\t}\n\telse if (result == DialogResult.No)\n\t{\n\t    // User clicked No\n\t}",
            DialogButtons.YesNo => "\tvar result = await dialogService.ShowAsync(XamlRoot, \"Message\", \"Title\", DialogButtons.YesNo);\n\tif (result == DialogResult.Yes)\n\t{\n\t    // User clicked Yes\n\t}\n\telse if (result == DialogResult.No)\n\t{\n\t    // User clicked No\n\t}",
            DialogButtons.RetryCancel => "\tvar result = await dialogService.ShowAsync(XamlRoot, \"Message\", \"Title\", DialogButtons.RetryCancel);\n\tif (result == DialogResult.Retry)\n\t{\n\t    // User clicked Retry\n\t}",
            DialogButtons.CancelTryContinue => "\tvar result = await dialogService.ShowAsync(XamlRoot, \"Message\", \"Title\", DialogButtons.CancelTryContinue);\n\tif (result == DialogResult.TryAgain)\n\t{\n\t    // User clicked Try Again\n\t}\n\telse if (result == DialogResult.Continue)\n\t{\n\t    // User clicked Continue\n\t}",
            _ => "\tawait dialogService.ShowAsync(XamlRoot, \"Message\", \"Title\", DialogButtons.OK);"
        };

        DifferentButtonsCSharpCode = string.Format(_differentButtonsMessageBoxSampleCSharpCodeString, content);
    }

    [ObservableProperty]
    public partial int SelectedImageIndex { get; set; } = 0;

    [ObservableProperty]
    public partial string DifferentImagesResult { get; set; } = "No image example shown yet";

    [ObservableProperty]
    public partial string DifferentImagesXamlCode { get; set; } = "<Button Content=\"Show MessageBox\" Click=\"ShowMessageButton_Click\" />";

    [ObservableProperty]
    public partial string DifferentImagesCSharpCode { get; set; } = string.Format(_differentImagesMessageBoxSampleCSharpCodeString, "\tawait dialogService.ShowAsync(XamlRoot, \"Message\", \"Title\", DialogButtons.OK, DialogImage.None);");

    partial void OnSelectedImageIndexChanged(int value)
    {
        UpdateImageCodeSnippets(value);
    }

    private void UpdateImageCodeSnippets(int index)
    {
        string content = index switch
        {
            0 => "\tawait dialogService.ShowAsync(XamlRoot, \"Message\", \"Title\", DialogButtons.OK, DialogImage.None);",
            1 => "\t// DialogImage.Error (also Hand, Stop)\n\tawait dialogService.ShowAsync(XamlRoot, \"An error occurred!\", \"Error\", DialogButtons.OK, DialogImage.Error);",
            2 => "\t// DialogImage.Question\n\tvar result = await dialogService.ShowAsync(XamlRoot, \"Do you want to continue?\", \"Question\", DialogButtons.YesNo, DialogImage.Question);",
            3 => "\t// DialogImage.Warning (also Exclamation)\n\tawait dialogService.ShowAsync(XamlRoot, \"Warning: This action may have consequences.\", \"Warning\", DialogButtons.OKCancel, DialogImage.Warning);",
            4 => "\t// DialogImage.Information (also Asterisk)\n\tawait dialogService.ShowAsync(XamlRoot, \"Operation completed successfully.\", \"Information\", DialogButtons.OK, DialogImage.Information);",
            _ => "\tawait dialogService.ShowAsync(XamlRoot, \"Message\", \"Title\", DialogButtons.OK, DialogImage.None);"
        };

        DifferentImagesCSharpCode = string.Format(_differentImagesMessageBoxSampleCSharpCodeString, content);
    }

    [ObservableProperty]
    public partial string CommonMessagesResult { get; set; } = "No common message shown yet";

    [ObservableProperty]
    public partial string CommonMessagesXamlCode { get; set; } = @"<WrapPanel Margin=""0,0,0,10"">
    <Button Content=""Information"" Click=""ShowInformationButton_Click"" />
    <Button Content=""Error"" Click=""ShowErrorButton_Click"" />
    <Button Content=""Warning"" Click=""ShowWarningButton_Click"" />
</WrapPanel>";

    [ObservableProperty]
    public partial string CommonMessagesCSharpCode { get; set; } = @"// Information
private void ShowInformationButton_Click(object sender, RoutedEventArgs e)
{
    await dialogService.ShowAsync(XamlRoot, ""Operation completed successfully."", ""Information"", DialogButtons.OK, DialogImage.Information);
}

// Error
private void ShowErrorButton_Click(object sender, RoutedEventArgs e)
{
    await dialogService.ShowAsync(XamlRoot, ""An error occurred!"", ""Error"", DialogButtons.OK, DialogImage.Error);
}

// Warning
private void ShowWarningButton_Click(object sender, RoutedEventArgs e)
{
    await dialogService.ShowAsync(XamlRoot, ""This action cannot be undone!"", ""Warning"", DialogButtons.OKCancel, DialogImage.Warning);
}";

    [ObservableProperty]
    public partial string CustomDefaultResult { get; set; } = "No selection made";

    private const string _differentButtonsMessageBoxSampleCSharpCodeString = "private void ShowDialogButtons_Click(object sender, RoutedEventArgs e)\n{{\n{0}\n}}";
    private const string _differentImagesMessageBoxSampleCSharpCodeString = "private void ShowDialogButtons_Click(object sender, RoutedEventArgs e)\n{{\n{0}\n}}";
}
