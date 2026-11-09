namespace WPFGallery.Models
{
    /// <summary>
    /// IconData class for icons in icon page
    /// </summary>
    public class IconData
    {
        public required string Name { get; set; }
        public required string Code { get; set; }
        public List<string> Tags { get; set; } = [];

        public string Character => char.ConvertFromUtf32(Convert.ToInt32(Code, 16));
        public string CodeGlyph => "\\x" + Code;
        public string TextGlyph => "&#x" + Code + ";";
        public string XamlCode => $"<FontIcon FontFamily=\"Segoe Fluent Icons\" Glyph=\"{TextGlyph}\" />";
    }
}
