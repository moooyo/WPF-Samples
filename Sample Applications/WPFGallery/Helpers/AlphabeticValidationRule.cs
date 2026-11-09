namespace WPFGallery.Helpers;

public static class AlphabeticValidationRule
{
    public static bool IsValid(string input) =>
        input.All(character => character is >= 'a' and <= 'z' or >= 'A' and <= 'Z');
}
