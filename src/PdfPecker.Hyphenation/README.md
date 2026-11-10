![PdfPecker](https://raw.githubusercontent.com/Moonwalker1307/PdfPecker/main/assets/logo.png)

# PdfPecker.Hyphenation

Hyphenation patterns for [PdfPecker](https://www.nuget.org/packages/PdfPecker) in 48 more languages and variants, for
CSS `hyphens: auto`, with one call. They come from the hyph-utf8 project, the patterns TeX uses.

```csharp
var options = new PdfOptions().AddHyphenationLanguages();
byte[] pdf = HtmlToPdf.Convert("<p lang='fi' style='hyphens: auto'>lentokonesuihkuturbiinimoottori</p>", options);
```

- Adds Afrikaans, Basque, Belarusian, Bulgarian, Catalan, Church Slavonic, Coptic, Croatian, Esperanto, Estonian,
  Ethiopic (Amharic, Tigrinya, Ge'ez), Finnish, Friulan, Galician, Georgian, German in traditional spelling (de-1901,
  de-CH-1901), Greek (monotonic as el, polytonic, ancient), Icelandic, Interlingua, Irish, Kazakh, Kurmanji, Latin
  (with classical and liturgical variants), Lithuanian, Mongolian, Occitan, Pali, Piedmontese, Romansh, Russian,
  Sanskrit, Serbo-Croatian (sr and sh, Cyrillic and Latin), Slovak, Slovenian, Albanian, Turkish, Turkmen, Ukrainian,
  Upper Sorbian, Welsh, British English (en-GB) and pinyin. `HyphenationLanguages.Tags` lists the tags.
- PdfPecker's own languages (American English, Danish, German, Dutch, Norwegian, Spanish, Swedish, French, Italian,
  Portuguese and Polish) are not repeated, and patterns already in `PdfOptions.Hyphenation` are kept.
- Nothing is read when the patterns are added. A language's patterns are read and compiled the first time a document
  hyphenates text in it, and shared by every conversion in the process.
- Works with trimming and Native AOT. The package has PdfPecker's version and needs that PdfPecker release.

Each pattern file is an unmodified copy under its own licence (MIT, LPPL and other permissive ones); see
THIRD-PARTY-NOTICES.md in the package.
