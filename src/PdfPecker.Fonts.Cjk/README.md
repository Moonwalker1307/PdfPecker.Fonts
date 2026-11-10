![PdfPecker](https://raw.githubusercontent.com/Moonwalker1307/PdfPecker/main/assets/logo.png)

# PdfPecker.Fonts.Cjk

Noto Sans CJK for [PdfPecker](https://www.nuget.org/packages/PdfPecker): Chinese (simplified, traditional and Hong
Kong), Japanese and Korean text in regular and bold, with one call.

```csharp
var options = new PdfOptions();
options.Fonts.AddNotoSansCjk();        // or "zh-Hant", "zh-HK", "ja", "ko"
byte[] pdf = HtmlToPdf.Convert("<p>你好，世界</p>", options);
```

- CJK text that a document's own fonts lack falls back to Noto Sans CJK; no CSS change is needed. The language you pass
  picks the regional glyph forms of that fallback (simplified Chinese by default).
- CSS can name any region: `"Noto Sans CJK SC"`, `"TC"`, `"HK"`, `"JP"`, `"KR"`, the Google Fonts names
  `"Noto Sans SC"` and so on, and `"Noto Sans Mono CJK JP"` and the other monospaced families.
- Nothing is read when the fonts are registered. The first document with CJK text reads one weight's collection
  (about 20 MB, kept once for the process), and the PDF embeds only the glyphs it uses.
- Works with trimming and Native AOT. The package has PdfPecker's version and needs that PdfPecker release.

The fonts are © Adobe, under the SIL Open Font License 1.1; see THIRD-PARTY-NOTICES.md in the package. More packages
(serif CJK, emoji, other scripts, hyphenation) are in the [PdfPecker.Fonts](https://github.com/Moonwalker1307/PdfPecker.Fonts)
repository.
