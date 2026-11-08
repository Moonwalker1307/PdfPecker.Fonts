namespace PdfPecker.FontPacks;

/// <summary>
/// A font face of an embedded file, as tools/FontPack reads it from the font: its family, weight, style, index in a
/// collection and the characters it has, so that it can be registered without reading the font.
/// </summary>
internal sealed record Face(string File, int Index, string Family, int Weight, bool Italic, string UnicodeRange)
{
    /// <summary>Registers the face under its own family name, or another one such as a Google Fonts alias.</summary>
    public void AddTo(FontCollection fonts, string? family = null)
        => fonts.AddDeferred(family ?? Family, EmbeddedFile.Loader(File), Weight, Italic, UnicodeRange, Index);
}
