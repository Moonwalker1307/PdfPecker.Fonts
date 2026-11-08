namespace PdfPecker.FontPacks;

/// <summary>
/// Registers the faces of a Noto CJK collection. The collections hold one family per region, JP, KR, SC, TC and HK,
/// which share most glyphs but draw some characters in their region's style. Font fallback takes the families in the
/// order they are registered, so the region of the chosen language comes first.
/// </summary>
internal static class CjkRegions
{
    private static readonly string[] Regions = ["SC", "TC", "HK", "JP", "KR"];

    /// <summary>The region of a language tag: zh-Hans, zh-CN and zh-SG are SC; zh-Hant and zh-TW TC; zh-HK and zh-MO HK.</summary>
    public static string Region(string language)
    {
        ArgumentNullException.ThrowIfNull(language);
        string tag = language.Trim().Replace('_', '-').ToLowerInvariant();
        string primary = tag.Split('-')[0];
        if (primary == "ja") return "JP";
        if (primary == "ko") return "KR";
        if (primary == "zh")
        {
            if (tag.Contains("-hant", StringComparison.Ordinal))
                return tag.Contains("-hk", StringComparison.Ordinal) || tag.Contains("-mo", StringComparison.Ordinal) ? "HK" : "TC";
            if (tag.Contains("-hans", StringComparison.Ordinal)) return "SC";
            if (tag.EndsWith("-hk", StringComparison.Ordinal) || tag.EndsWith("-mo", StringComparison.Ordinal)) return "HK";
            if (tag.EndsWith("-tw", StringComparison.Ordinal)) return "TC";
            return "SC";
        }
        throw new ArgumentException($"'{language}' is not a Chinese, Japanese or Korean language tag, such as zh-Hans, zh-Hant, zh-HK, ja or ko.", nameof(language));
    }

    /// <summary>
    /// Registers every face: the chosen region's family first, then the other regions, then the monospaced families
    /// (if the collection has them). Each region's family also gets its Google Fonts name, such as "Noto Sans SC", so
    /// that CSS written for Google Fonts finds it.
    /// </summary>
    public static FontCollection Add(FontCollection fonts, IReadOnlyList<Face> faces, string style, string language)
    {
        ArgumentNullException.ThrowIfNull(fonts);
        string first = Region(language);
        foreach (string region in Regions.OrderBy(r => r == first ? 0 : 1))
        {
            foreach (var face in faces.Where(f => f.Family == $"Noto {style} CJK {region}")) face.AddTo(fonts);
            foreach (var face in faces.Where(f => f.Family == $"Noto {style} CJK {region}")) face.AddTo(fonts, $"Noto {style} {region}");
        }
        foreach (var face in faces.Where(f => f.Family.StartsWith($"Noto {style} Mono CJK ", StringComparison.Ordinal))) face.AddTo(fonts);
        return fonts;
    }
}
