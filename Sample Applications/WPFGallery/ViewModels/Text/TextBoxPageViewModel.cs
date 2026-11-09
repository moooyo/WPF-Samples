using System.ComponentModel.DataAnnotations;
using DataAnnotationsValidationResult = System.ComponentModel.DataAnnotations.ValidationResult;

namespace WPFGallery.ViewModels
{
    public partial class TextBoxPageViewModel : ObservableObject
    {
        [ObservableProperty]
        public partial string PageTitle { get; set; } = "TextBox";

        [ObservableProperty]
        public partial string PageDescription { get; set; } = "";

        [ObservableProperty]
        public partial string ValidatedText { get; set; } = string.Empty;

        [ObservableProperty]
        public partial string InputText { get; set; } = string.Empty;

        [ObservableProperty]
        public partial string ValidationError { get; set; } = string.Empty;

        partial void OnInputTextChanged(string value)
        {
            if (Helpers.AlphabeticValidationRule.IsValid(value))
            {
                ValidatedText = value;
                ValidationError = string.Empty;
            }
            else
            {
                ValidationError = "Only English alphabetic characters (a-z, A-Z) are allowed.";
            }
        }
    }
}
