using System.Text;
using PdfPecker.FontPacks;

namespace PdfPecker;

/// <summary>
/// Hyphenation patterns from the hyph-utf8 project for languages that PdfPecker has no patterns of its own for, for
/// CSS <c>hyphens: auto</c>.
/// </summary>
public static class HyphenationLanguages
{
    // One set of patterns per file for the whole process, so that options made for each conversion share the patterns
    // that an earlier conversion read and compiled.
    private static readonly Dictionary<string, HyphenationPatterns> Shared = Languages.All
        .SelectMany(l =>
        {
            var patterns = HyphenationPatterns.Deferred(() => Encoding.UTF8.GetString(EmbeddedFile.Read(l.File)), l.Left, l.Right);
            return l.Tags.Select(tag => (tag, patterns));
        })
        .ToDictionary(x => x.tag, x => x.patterns, StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// The language tags the package has patterns for, such as "en-gb", "fi" or "uk". A tag also serves longer tags of
    /// its language, so "fi" covers "fi-FI".
    /// </summary>
    public static IReadOnlyList<string> Tags { get; } = Languages.All.SelectMany(l => l.Tags).ToArray();

    /// <summary>
    /// Adds the patterns of every language in <see cref="Tags"/> to <see cref="PdfOptions.Hyphenation"/>, creating it
    /// if it is null. Patterns already there for a tag are kept, so a caller's own patterns win.
    /// </summary>
    /// <remarks>
    /// Nothing is read when the patterns are added. A language's patterns are read and compiled the first time a
    /// document hyphenates text in it, and kept for the life of the process.
    /// </remarks>
    /// <param name="options">The options to add the patterns to.</param>
    /// <returns>The options, for chaining.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="options"/> is null.</exception>
    /// <example><code>var options = new PdfOptions().AddHyphenationLanguages();</code></example>
    public static PdfOptions AddHyphenationLanguages(this PdfOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        options.Hyphenation ??= new Dictionary<string, HyphenationPatterns>(StringComparer.OrdinalIgnoreCase);
        foreach (var (tag, patterns) in Shared) options.Hyphenation.TryAdd(tag, patterns);
        return options;
    }

    /// <summary>The patterns for a language tag in <see cref="Tags"/>, for a <see cref="PdfOptions.Hyphenation"/> of your own.</summary>
    /// <param name="tag">A tag from <see cref="Tags"/>, such as "fi"; case does not matter.</param>
    /// <exception cref="ArgumentNullException"><paramref name="tag"/> is null.</exception>
    /// <exception cref="ArgumentException">The package has no patterns for the tag.</exception>
    public static HyphenationPatterns Patterns(string tag)
    {
        ArgumentNullException.ThrowIfNull(tag);
        return Shared.TryGetValue(tag.Trim(), out var patterns) ? patterns
            : throw new ArgumentException($"There are no hyphenation patterns for '{tag}'.", nameof(tag));
    }
}
