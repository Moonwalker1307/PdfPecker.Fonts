# PdfPecker.Fonts.Scripts

Noto Sans for [PdfPecker](https://www.nuget.org/packages/PdfPecker) in 17 scripts that PdfPecker shapes but has no
font for, in regular and bold: Arabic, Hebrew, Thaana, Devanagari, Bengali, Gurmukhi, Gujarati, Oriya, Tamil, Telugu,
Kannada, Malayalam, Thai, Lao, Armenian, Georgian and Ethiopic.

```csharp
var options = new PdfOptions();
options.Fonts.AddNotoSansScripts();                 // all of them
options.Fonts.AddNotoSansScripts("Arabic", "Hebrew"); // or some
byte[] pdf = HtmlToPdf.Convert("<p dir='rtl'>مرحبا بالعالم</p><p>नमस्ते दुनिया</p>", options);
```

- Text in a script that a document's own fonts lack falls back to its Noto Sans family, such as `"Noto Sans Arabic"`;
  no CSS change is needed. `NotoSansScripts.Scripts` lists the script names.
- Nothing is read when the fonts are registered. A font is read the first time a document has its script (up to
  300 KB, kept once for the process), and the PDF embeds only the glyphs it uses.
- Sinhala, Khmer, Myanmar and Tibetan are left out until PdfPecker shapes them.
- Works with trimming and Native AOT. The package has PdfPecker's version and needs that PdfPecker release.

The fonts are © The Noto Project Authors, under the SIL Open Font License 1.1; see THIRD-PARTY-NOTICES.md in the package.
