# PdfPecker.Fonts.Emoji

Noto Color Emoji for [PdfPecker](https://www.nuget.org/packages/PdfPecker): colour emoji in PDFs, as vector COLRv1
glyphs, printed the way Chrome prints them.

```csharp
var options = new PdfOptions();
options.Fonts.AddNotoColorEmoji();
byte[] pdf = HtmlToPdf.Convert("<p>Release day 🎉 🚀 🇩🇰 👨‍👩‍👧 👍🏽</p>", options);
```

- Emoji that a document's own fonts lack fall back to it; no CSS change is needed. Flags, skin tones and joined
  sequences print as one glyph.
- Nothing is read when the font is registered. The first document with emoji reads it (about 5 MB, kept once for the
  process), and the PDF embeds only the emoji it uses, as vector graphics that stay sharp at any zoom.
- Works with trimming and Native AOT. The package has PdfPecker's version and needs that PdfPecker release.

The font is © Google, under the SIL Open Font License 1.1; see THIRD-PARTY-NOTICES.md in the package.
