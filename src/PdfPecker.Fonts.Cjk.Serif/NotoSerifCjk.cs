using PdfPecker.FontPacks;

namespace PdfPecker;

/// <summary>Noto Serif CJK, the serif (Song and Mincho style) Chinese, Japanese and Korean fonts of Google's Noto project.</summary>
public static class NotoSerifCjk
{
    /// <summary>
    /// Registers Noto Serif CJK in regular and bold: the families "Noto Serif CJK SC", "TC", "HK", "JP" and "KR", also
    /// under their Google Fonts names, "Noto Serif SC" and so on. Text in CJK characters that a document's own fonts
    /// lack falls back to them; name a family in CSS <c>font-family</c> to choose them over a sans-serif CJK font.
    /// </summary>
    /// <remarks>
    /// Nothing is read when the fonts are registered. The first document that needs them reads one weight's collection
    /// (about 27 MB, kept for the life of the process), and the PDF embeds only the glyphs it uses.
    /// </remarks>
    /// <param name="fonts">The collection, usually <see cref="PdfOptions.Fonts"/>.</param>
    /// <param name="language">
    /// The language whose regional glyph forms fallback text gets, as a language tag: <c>zh-Hans</c> (the default) for
    /// simplified Chinese, <c>zh-Hant</c> or <c>zh-TW</c> for traditional Chinese, <c>zh-HK</c> for Hong Kong,
    /// <c>ja</c> for Japanese or <c>ko</c> for Korean. CSS can still pick any region's family by name.
    /// </param>
    /// <returns>The collection, for chaining.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="fonts"/> or <paramref name="language"/> is null.</exception>
    /// <exception cref="ArgumentException"><paramref name="language"/> is not a Chinese, Japanese or Korean tag.</exception>
    /// <example><code>options.Fonts.AddNotoSerifCjk("zh-Hant");</code></example>
    public static FontCollection AddNotoSerifCjk(this FontCollection fonts, string language = "zh-Hans")
        => CjkRegions.Add(fonts, Faces.All, "Serif", language);
}
