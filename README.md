![PdfPecker](https://raw.githubusercontent.com/Moonwalker1307/PdfPecker/main/assets/logo.png)

# PdfPecker.Fonts

Opt-in fonts and hyphenation patterns for [PdfPecker](https://github.com/Moonwalker1307/PdfPecker), the
dependency-free HTML-to-PDF library for .NET. PdfPecker itself bundles only Latin, Greek and Cyrillic fonts and
hyphenation in eleven languages, because one CJK font weight alone is about 20 MB. These packages add the rest, one
call each:

| Package | What it adds | Package size |
|---|---|---|
| [PdfPecker.Fonts.Cjk](src/PdfPecker.Fonts.Cjk) | Noto Sans CJK, regular and bold: Chinese (simplified, traditional, Hong Kong), Japanese, Korean | 32 MB |
| [PdfPecker.Fonts.Cjk.Serif](src/PdfPecker.Fonts.Cjk.Serif) | Noto Serif CJK, regular and bold, in the same five regions | 44 MB |
| [PdfPecker.Fonts.Emoji](src/PdfPecker.Fonts.Emoji) | Noto Color Emoji as vector COLRv1 glyphs, with flags, skin tones and joined sequences | 3 MB |
| [PdfPecker.Fonts.Scripts](src/PdfPecker.Fonts.Scripts) | Noto Sans in 17 scripts, regular and bold: Arabic, Hebrew, Thaana, Devanagari, Bengali, Gurmukhi, Gujarati, Oriya, Tamil, Telugu, Kannada, Malayalam, Thai, Lao, Armenian, Georgian, Ethiopic | 1.5 MB |
| [PdfPecker.Hyphenation](src/PdfPecker.Hyphenation) | `hyphens: auto` in 48 more languages and variants, such as British English, Finnish, Russian, Ukrainian, Greek and Catalan | 0.9 MB |

```csharp
using PdfPecker;

var options = new PdfOptions().AddHyphenationLanguages();
options.Fonts
    .AddNotoSansCjk("ja")      // Japanese glyph forms for CJK text that falls back
    .AddNotoColorEmoji()
    .AddNotoSansScripts();

byte[] pdf = HtmlToPdf.Convert("<p>こんにちは 🎉 مرحبا नमस्ते</p>", options);
```

No CSS change is needed: text in characters that a document's own fonts lack falls back to these fonts, as a browser
falls back to the system's fonts. CSS can also name them, as `font-family: "Noto Sans CJK JP"` or by their Google Fonts
names such as `"Noto Sans JP"` and `"Noto Serif TC"`.

## Fast and light until used

- **Registering costs nothing.** The calls only record each face's name, weight and the characters it has (generated
  from the fonts at build time). Registering every package takes under 2 ms once the code is warm and reads no font.
- **A font is read the first time a document needs it,** and kept once for the whole process, shared by every
  `PdfOptions` and conversion. A CJK weight is about 20 MB in memory; a script font up to 300 KB.
- **Fallback skips fonts that can't help.** A character outside a font's characters never loads that font, so Arabic
  text never reads a CJK font.
- **PDFs embed only the glyphs they use.** A page of Chinese text adds about 200 KB per CJK face it uses.
- **Trimming and Native AOT** work: the fonts are embedded resources and nothing uses reflection. A Native AOT app
  that registers four packages and converts CJK, Arabic, Devanagari and emoji text starts, loads and converts in about
  65 ms, with a peak of about 105 MB.

The packages target .NET 8 (and so run on .NET 8, 9 and 10), carry no dependency besides PdfPecker, and have the
version of the PdfPecker release they need.

## What is left out

- Sinhala, Khmer, Myanmar and Tibetan: PdfPecker does not shape these scripts yet, so their fonts would print wrong.
- Hyphenation for Indic scripts and Thai: PdfPecker's hyphenation does not handle their vowel signs or line breaks yet.
- Patterns only available under the GPL or LGPL (Czech, Hungarian, Latvian, Armenian, Indonesian, Macedonian, Serbian
  Cyrillic) and files that grant no licence (Romanian, Mongolian LMC).
- Weights other than regular and bold. CSS weights pick the nearer of the two, as browsers do.

## Licences

The code is MIT. The fonts are under the SIL Open Font License 1.1 and the hyphenation patterns under their own
permissive licences (MIT, LPPL and others); [THIRD-PARTY-NOTICES.md](THIRD-PARTY-NOTICES.md) lists every file, its
source commit and licence. Every file is embedded unmodified.

## Building

You need the .NET 10 SDK. The fonts are not in git: the first command downloads them (about 100 MB) from commits that
`sources.json` pins, and checks each file's SHA-256.

```
dotnet run --project tools/FontPack -- fetch
dotnet build -c Release
dotnet test -c Release
```

`dotnet run --project tools/FontPack -- generate` rewrites the generated face and language lists after a change to
`sources.json`; CI fails when they are out of date. [AGENTS.md](AGENTS.md) has the repository's rules and
[RELEASING.md](RELEASING.md) the release steps.
