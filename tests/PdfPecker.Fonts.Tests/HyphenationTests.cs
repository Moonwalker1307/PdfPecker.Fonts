using Xunit;

namespace PdfPecker.Fonts.Tests;

/// <summary>The hyphenation package's languages: each file's patterns load and break a long word.</summary>
public class HyphenationTests
{
    // A long word in a narrow column.
    public static TheoryData<string, string> Words => new()
    {
        { "af", "onafhanklikheidsverklaring" }, { "be", "беларускамоўны" }, { "bg", "противоконституционно" },
        { "ca", "internacionalització" }, { "cy", "llongyfarchiadau" }, { "de-1901", "Schiffahrtsgesellschaft" },
        { "el", "αντισυνταγματικότητα" }, { "en-gb", "internationalisation" }, { "eo", "malsanulejoj" },
        { "et", "kirjandusteadlane" }, { "eu", "ezinbestekoak" }, { "fi", "lentokonesuihkuturbiinimoottoriapumekaanikko" },
        { "ga", "neamhspleáchas" }, { "gl", "internacionalización" }, { "hr", "prijestolonasljednik" },
        { "is", "þjóðhátíðardagurinn" }, { "ka", "დამოუკიდებლობა" }, { "la", "conservationem" },
        { "lt", "nebeprisikiškiakopūsteliaudavome" }, { "mn", "монголчуудын" }, { "ru", "достопримечательности" },
        { "sk", "najneobhospodarovávateľnejšie" }, { "sl", "dolgočasnost" }, { "sr", "непријатељство" },
        { "tr", "çekoslovakyalılaştıramadıklarımızdanmışsınız" },
        { "uk", "найрозповсюдженіший" },
    };

    [Theory]
    [MemberData(nameof(Words))]
    public void PatternsBreakALongWord(string tag, string word)
    {
        Assert.Contains(tag, HyphenationLanguages.Tags);
        var options = new PdfOptions().AddHyphenationLanguages();
        options.Fonts.AddNotoSansScripts();
        var pdf = HtmlToPdf.Convert($"<p lang='{tag}' style='width: 40px; hyphens: auto; font: 12px sans-serif'>{word} {word}</p>", options);
        Assert.Contains("-", PdfText.Extract(pdf));
    }

    [Fact]
    public void EveryFileLoads()
    {
        foreach (var tag in HyphenationLanguages.Tags)
        {
            var options = new PdfOptions { Hyphenation = new Dictionary<string, HyphenationPatterns> { [tag] = HyphenationLanguages.Patterns(tag) } };
            HtmlToPdf.Convert($"<p lang='{tag}' style='width: 40px; hyphens: auto'>abcdefghijklmnop</p>", options);
        }
    }

    [Fact]
    public void CallersOwnPatternsAreKept()
    {
        var mine = new HyphenationPatterns("c1d");
        var options = new PdfOptions { Hyphenation = new Dictionary<string, HyphenationPatterns> { ["fi"] = mine } };
        options.AddHyphenationLanguages();
        Assert.Same(mine, options.Hyphenation!["fi"]);
        Assert.Same(HyphenationLanguages.Patterns("ru"), options.Hyphenation["ru"]);
        Assert.Same(HyphenationLanguages.Patterns("RU"), new PdfOptions().AddHyphenationLanguages().Hyphenation!["ru"]);
        Assert.Throws<ArgumentException>(() => HyphenationLanguages.Patterns("hu"));
        Assert.DoesNotContain("en-us", HyphenationLanguages.Tags); // the library has its own
    }
}
