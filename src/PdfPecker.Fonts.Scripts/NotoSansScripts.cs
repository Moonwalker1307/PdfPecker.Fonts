using PdfPecker.FontPacks;

namespace PdfPecker;

/// <summary>
/// Noto Sans fonts for scripts that PdfPecker shapes but has no font for: right-to-left, Indic, Southeast Asian and
/// other alphabets.
/// </summary>
public static class NotoSansScripts
{
    private const string Prefix = "Noto Sans ";

    /// <summary>
    /// The scripts the package has a font for, by the name that follows "Noto Sans " in the family name, such as
    /// "Arabic" or "Devanagari".
    /// </summary>
    public static IReadOnlyList<string> Scripts { get; } = Faces.All.Select(f => f.Family[Prefix.Length..]).Distinct().ToArray();

    /// <summary>
    /// Registers every script's Noto Sans family in regular and bold, such as "Noto Sans Arabic". Text in a script that a
    /// document's own fonts lack falls back to its family, so no CSS change is needed.
    /// </summary>
    /// <remarks>
    /// Nothing is read when the fonts are registered. A font is read the first time a document has its script (up to
    /// 300 KB each, kept for the life of the process), and the PDF embeds only the glyphs it uses.
    /// </remarks>
    /// <param name="fonts">The collection, usually <see cref="PdfOptions.Fonts"/>.</param>
    /// <returns>The collection, for chaining.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="fonts"/> is null.</exception>
    /// <example><code>options.Fonts.AddNotoSansScripts();</code></example>
    public static FontCollection AddNotoSansScripts(this FontCollection fonts)
    {
        ArgumentNullException.ThrowIfNull(fonts);
        foreach (var face in Faces.All) face.AddTo(fonts);
        return fonts;
    }

    /// <summary>
    /// Registers the Noto Sans families of some scripts, named as in <see cref="Scripts"/> (case does not matter), in
    /// the order given, which is the order fallback tries them in.
    /// </summary>
    /// <param name="fonts">The collection, usually <see cref="PdfOptions.Fonts"/>.</param>
    /// <param name="scripts">Script names such as "Arabic" or "Thai".</param>
    /// <returns>The collection, for chaining.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="fonts"/> or <paramref name="scripts"/> is null.</exception>
    /// <exception cref="ArgumentException">A name is not one of <see cref="Scripts"/>.</exception>
    /// <example><code>options.Fonts.AddNotoSansScripts("Arabic", "Hebrew");</code></example>
    public static FontCollection AddNotoSansScripts(this FontCollection fonts, params string[] scripts)
    {
        ArgumentNullException.ThrowIfNull(fonts);
        ArgumentNullException.ThrowIfNull(scripts);
        var families = new List<string>(scripts.Length);
        foreach (var script in scripts)
        {
            string? name = script == null ? null : Scripts.FirstOrDefault(s => string.Equals(s, script.Trim(), StringComparison.OrdinalIgnoreCase));
            if (name == null)
                throw new ArgumentException($"There is no font for the script '{script}'. The scripts are: {string.Join(", ", Scripts)}.", nameof(scripts));
            families.Add(Prefix + name);
        }
        foreach (var family in families.Distinct(StringComparer.Ordinal))
            foreach (var face in Faces.All.Where(f => f.Family == family)) face.AddTo(fonts);
        return fonts;
    }
}
