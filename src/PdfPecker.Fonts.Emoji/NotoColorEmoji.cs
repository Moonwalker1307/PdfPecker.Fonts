using PdfPecker.FontPacks;

namespace PdfPecker;

/// <summary>Noto Color Emoji, the colour emoji font of Google's Noto project, in its vector COLRv1 version.</summary>
public static class NotoColorEmoji
{
    /// <summary>
    /// Registers the family "Noto Color Emoji". Emoji that a document's own fonts lack fall back to it, so no CSS change
    /// is needed; flags, skin tones and joined sequences such as 👨‍👩‍👧 print as one glyph.
    /// </summary>
    /// <remarks>
    /// Nothing is read when the font is registered. The first document with emoji reads it (about 5 MB, kept for the
    /// life of the process), and the PDF embeds only the emoji it uses, as vector graphics.
    /// </remarks>
    /// <param name="fonts">The collection, usually <see cref="PdfOptions.Fonts"/>.</param>
    /// <returns>The collection, for chaining.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="fonts"/> is null.</exception>
    /// <example><code>options.Fonts.AddNotoColorEmoji();</code></example>
    public static FontCollection AddNotoColorEmoji(this FontCollection fonts)
    {
        ArgumentNullException.ThrowIfNull(fonts);
        foreach (var face in Faces.All) face.AddTo(fonts);
        return fonts;
    }
}
