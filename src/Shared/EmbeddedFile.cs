using System.Collections.Concurrent;

namespace PdfPecker.FontPacks;

/// <summary>
/// The font and pattern files a package embeds. A file is read into memory the first time a document needs it and then
/// kept for the life of the process, so that every <see cref="FontCollection"/> that registers the package shares one
/// copy instead of reading tens of megabytes again for each.
/// </summary>
internal static class EmbeddedFile
{
    private static readonly ConcurrentDictionary<string, Lazy<byte[]>> Files = new(StringComparer.Ordinal);

    /// <summary>A loader for <see cref="FontCollection.AddDeferred"/> that returns the shared copy of a file.</summary>
    public static Func<byte[]> Loader(string name)
    {
        var file = Files.GetOrAdd(name, static n => new Lazy<byte[]>(() => Read(n)));
        return () => file.Value;
    }

    /// <summary>
    /// The file's bytes. The resource stream reads straight from the assembly image, which the runtime maps into memory,
    /// so the one copy is the array itself.
    /// </summary>
    public static byte[] Read(string name)
    {
        using var stream = typeof(EmbeddedFile).Assembly.GetManifestResourceStream(name)
            ?? throw new IOException($"The package has no embedded file {name}.");
        var data = GC.AllocateUninitializedArray<byte>(checked((int)stream.Length));
        stream.ReadExactly(data);
        return data;
    }
}
