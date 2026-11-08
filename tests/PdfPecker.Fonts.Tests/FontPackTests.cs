using System.Text;
using Xunit;

namespace PdfPecker.Fonts.Tests;

/// <summary>Each package's registration, its fallback for the characters of its fonts, and what a PDF embeds.</summary>
public class FontPackTests
{
    private static PdfOptions Options() => new() { Compress = false };

    private static string Latin1(byte[] pdf) => Encoding.Latin1.GetString(pdf);

    [Fact]
    public void RegisteringReadsNoFont()
    {
        long before = GC.GetAllocatedBytesForCurrentThread();
        var fonts = new FontCollection().AddNotoSansCjk().AddNotoSerifCjk().AddNotoColorEmoji().AddNotoSansScripts();
        long allocated = GC.GetAllocatedBytesForCurrentThread() - before;
        // The unicode ranges are parsed (about 3 MB for 85 faces, garbage at once); the fonts themselves, over 90 MB, are not read.
        Assert.True(allocated < 5_000_000, $"{allocated} bytes allocated");
        Assert.Equal(30 + 20 + 1 + 34, fonts.Count);
    }

    [Fact]
    public void CjkTextFallsBackToTheChosenRegion()
    {
        var options = Options();
        options.Fonts.AddNotoSansCjk("ja");
        var pdf = HtmlToPdf.Convert("<p>日本語のテキスト <b>太字</b></p>", options);
        var text = Latin1(pdf);
        Assert.Contains("NotoSansCJKjp-Regular", text);
        Assert.Contains("NotoSansCJKjp-Bold", text);
        Assert.Contains("日本語のテキスト", PdfText.Extract(pdf));
        // Only the used glyphs are embedded, not the 20 MB collection.
        Assert.True(pdf.Length < 1_500_000, $"{pdf.Length} bytes");
    }

    [Theory]
    [InlineData("zh-Hans", "sc")]
    [InlineData("zh-CN", "sc")]
    [InlineData("zh", "sc")]
    [InlineData("zh-Hant", "tc")]
    [InlineData("zh-TW", "tc")]
    [InlineData("zh-HK", "hk")]
    [InlineData("zh-Hant-HK", "hk")]
    [InlineData("ko-KR", "kr")]
    public void LanguageTagsPickTheRegion(string language, string region)
    {
        var options = Options();
        options.Fonts.AddNotoSansCjk(language);
        Assert.Contains($"NotoSansCJK{region}-Regular", Latin1(HtmlToPdf.Convert("<p>中文</p>", options)));
    }

    [Fact]
    public void CssCanNameEveryRegionAndTheGoogleFontsNames()
    {
        var options = Options();
        options.Fonts.AddNotoSansCjk().AddNotoSerifCjk();
        Assert.Contains("Noto Sans CJK KR", options.Fonts.FamilyNames);
        Assert.Contains("Noto Sans JP", options.Fonts.FamilyNames);
        Assert.Contains("Noto Serif TC", options.Fonts.FamilyNames);
        var text = Latin1(HtmlToPdf.Convert(
            "<p style='font-family: \"Noto Serif JP\"'>明朝</p><p style='font-family: \"Noto Sans Mono CJK KR\"'>한국어</p>", options));
        Assert.Contains("NotoSerifCJKjp-Regular", text);
        Assert.Contains("NotoSansMonoCJKkr-Regular", text);
    }

    [Fact]
    public void UnknownLanguagesAreRejected()
    {
        var fonts = new FontCollection();
        Assert.Throws<ArgumentException>(() => fonts.AddNotoSansCjk("de"));
        Assert.Throws<ArgumentNullException>(() => fonts.AddNotoSerifCjk(null!));
        Assert.Throws<ArgumentNullException>(() => NotoSansCjk.AddNotoSansCjk(null!));
        Assert.Equal(0, fonts.Count);
    }

    [Fact]
    public void EmojiPrintInColour()
    {
        var options = Options();
        options.Fonts.AddNotoColorEmoji();
        var text = Latin1(HtmlToPdf.Convert("<p>Party 🎉 flag 🇩🇰 family 👨‍👩‍👧</p>", options));
        Assert.Contains("/Type3", text);
    }

    [Theory]
    [InlineData("Arabic", "مرحبا بالعالم")]
    [InlineData("Hebrew", "שלום עולם")]
    [InlineData("Thaana", "ދިވެހި")]
    [InlineData("Devanagari", "नमस्ते दुनिया")]
    [InlineData("Bengali", "হ্যালো বিশ্ব")]
    [InlineData("Gurmukhi", "ਸਤ ਸ੍ਰੀ ਅਕਾਲ")]
    [InlineData("Gujarati", "નમસ્તે વિશ્વ")]
    [InlineData("Oriya", "ନମସ୍କାର")]
    [InlineData("Tamil", "வணக்கம் உலகம்")]
    [InlineData("Telugu", "హలో ప్రపంచం")]
    [InlineData("Kannada", "ನಮಸ್ಕಾರ ಜಗತ್ತು")]
    [InlineData("Malayalam", "ഹലോ വേൾഡ്")]
    [InlineData("Thai", "สวัสดีชาวโลก")]
    [InlineData("Lao", "ສະບາຍດີ")]
    [InlineData("Armenian", "Բարեւ աշխարհ")]
    [InlineData("Georgian", "გამარჯობა")]
    [InlineData("Ethiopic", "ሰላም ልዑል")]
    public void EachScriptFallsBackToItsFont(string script, string sample)
    {
        Assert.Contains(script, NotoSansScripts.Scripts);
        var options = Options();
        options.Fonts.AddNotoSansScripts();
        var text = Latin1(HtmlToPdf.Convert($"<p>{sample}</p><p><b>{sample}</b></p>", options));
        Assert.Contains($"NotoSans{script}-Regular", text);
        Assert.Contains($"NotoSans{script}-Bold", text);
    }

    [Fact]
    public void SomeScriptsCanBeChosen()
    {
        var fonts = new FontCollection().AddNotoSansScripts("arabic", " Hebrew ", "Arabic");
        Assert.Equal(4, fonts.Count);
        Assert.Equal(["Noto Sans Arabic", "Noto Sans Hebrew"], fonts.FamilyNames.Order(StringComparer.Ordinal));
        Assert.Throws<ArgumentException>(() => fonts.AddNotoSansScripts("Klingon"));
        Assert.Equal(17, NotoSansScripts.Scripts.Count);
        Assert.DoesNotContain("Sinhala", NotoSansScripts.Scripts); // PdfPecker does not shape it yet
    }

    [Fact]
    public void PacksWorkTogether()
    {
        var options = Options();
        options.Fonts.AddNotoSansScripts().AddNotoSansCjk().AddNotoColorEmoji();
        var pdf = HtmlToPdf.Convert("<p>Hello مرحبا 你好 🎉 สวัสดี</p>", options);
        var text = Latin1(pdf);
        foreach (var name in new[] { "NotoSansArabic", "NotoSansCJKsc", "NotoSansThai" }) Assert.Contains(name, text);
    }
}
