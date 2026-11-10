![PdfPecker](https://raw.githubusercontent.com/Moonwalker1307/PdfPecker/main/assets/logo.png)

# PdfPecker.Fonts.Cjk.Serif

Noto Serif CJK for [PdfPecker](https://www.nuget.org/packages/PdfPecker): the Song and Mincho style serif fonts for
Chinese (simplified, traditional and Hong Kong), Japanese and Korean, in regular and bold, with one call.

```csharp
var options = new PdfOptions();
options.Fonts.AddNotoSerifCjk("ja");
byte[] pdf = HtmlToPdf.Convert("<p style='font-family: \"Noto Serif JP\"'>明朝体のテキスト</p>", options);
```

- CSS names a region's family: `"Noto Serif CJK SC"`, `"TC"`, `"HK"`, `"JP"`, `"KR"`, or the Google Fonts names
  `"Noto Serif SC"` and so on. CJK text that a document's own fonts lack also falls back to it, in the regional forms of
  the language you pass; register PdfPecker.Fonts.Cjk first to have sans-serif as the fallback instead.
- Nothing is read when the fonts are registered. The first document that uses them reads one weight's collection
  (about 27 MB, kept once for the process), and the PDF embeds only the glyphs it uses.
- Works with trimming and Native AOT. The package has PdfPecker's version and needs that PdfPecker release.

The fonts are © Adobe, under the SIL Open Font License 1.1; see THIRD-PARTY-NOTICES.md in the package.
