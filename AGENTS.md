# AGENTS.md

Guidance for coding agents (and people) working in this repository. `CLAUDE.md` imports this file, so Claude Code and
other agents read the same rules.

PdfPecker.Fonts holds opt-in fonts and hyphenation patterns for [PdfPecker](https://github.com/Moonwalker1307/PdfPecker),
kept out of the library because they are large: one CJK font weight is about 20 MB. There are five NuGet packages:
PdfPecker.Fonts.Cjk (Noto Sans CJK), PdfPecker.Fonts.Cjk.Serif (Noto Serif CJK), PdfPecker.Fonts.Emoji (Noto Color
Emoji, COLRv1), PdfPecker.Fonts.Scripts (Noto Sans in 17 scripts) and PdfPecker.Hyphenation (hyph-utf8 patterns). Each
is a few lines of C# over embedded files, registered with PdfPecker's public `FontCollection.AddDeferred` and
`HyphenationPatterns.Deferred`.

## Read first

- `README.md`: what the packages add, what they leave out, and how to build.
- `RELEASING.md`: versioning, building against an unreleased library, and updating a font or pattern file.
- The library's `AGENTS.md` for how PdfPecker resolves fonts, falls back and hyphenates.

## Core rules

- **PdfPecker comes from NuGet.** Each package references the PdfPecker version in `PdfPeckerVersion`, which is also
  the packages' version. Never copy library code here. When the packages need something PdfPecker cannot do, fix the
  library in its own repository and pull request first; until its release, CI builds against the library's main
  branch, and a release refuses to.
- **No dependencies.** The only package reference in `src/` is PdfPecker. Tests may use xUnit.
- **Pinned, unmodified files.** Every font and pattern file is listed in `sources.json` with a URL at a commit and its
  SHA-256, downloaded by `tools/FontPack fetch` and embedded byte for byte. Never commit the files themselves, never
  point a URL at a branch, and never alter a file (the LPPL patterns may only be distributed unmodified).
- **Generated lists.** `src/*/Faces.g.cs` and `src/PdfPecker.Hyphenation/Languages.g.cs` come from
  `tools/FontPack generate`; change `sources.json` or the tool, never the output. CI runs `check`.
- **Only permissive licences.** Fonts under the SIL Open Font License; patterns under MIT, LPPL, BSD-style or other
  permissive terms. Nothing that is only GPL or LGPL, and nothing without a licence. Every file is listed with its
  licence in `THIRD-PARTY-NOTICES.md`.
- **Nothing is read until used.** Registering must stay cheap (under 2 ms for every package together): no font or
  pattern is read at registration, files are read once per process through `EmbeddedFile`, and every face has its
  unicode-range so fallback never loads a font that cannot help.
- **Only scripts PdfPecker renders correctly.** A font for a script the library cannot shape (Sinhala, Khmer, Myanmar,
  Tibetan today) or patterns it cannot apply (Indic, Thai) stay out until the library handles them; check new scripts
  against Chrome with the same fonts installed.
- **No breaking API changes before 2.0**, as in the library: add methods and overloads, don't rename or remove them.
- **Trimmable and AOT-compatible.** No reflection. Classes are `sealed` or `static`; `.globalconfig` turns the .NET
  performance and culture analyzers into build errors, as in PdfPecker.
- **One target framework.** The packages target net8.0 only, so the fonts are in each package once.
  1.25.0 is the last release for .NET 8 and 9 (`README.md`), as in PdfPecker; the release after it targets net10.0 only.
- **Never name a user's private application or employer** in code, tests, commits, the README or PR descriptions.

## Repo map

- `sources.json`: every embedded file, its package, URL and SHA-256 (hyphenation entries also have their language
  tags and hyphen minimums).
- `tools/FontPack/`: `fetch`, `generate` and `check`; reads each font's names, weight, style and cmap.
- `src/Directory.Build.props`: what the packages share, including embedding each `SourceFile` from `sources/`.
- `src/Shared/`: `EmbeddedFile` (one shared copy of each file per process) and `Face` (a face and its registration);
  `src/SharedCjk/`: the region order and Google Fonts names of the two CJK packages.
- `src/PdfPecker.Fonts.*/` and `src/PdfPecker.Hyphenation/`: the public extension methods, the generated list and the
  package README (its nuget.org page).
- `tests/PdfPecker.Fonts.Tests/`: xUnit over the public API: registration cost, fallback per script and region,
  emoji, every hyphenation file.
- `.github/workflows/build.yml`: fetch, check, build, test, pack; publishes on a published release.
- `.github/workflows/vulnerability-scan.yml`: a daily OSV-Scanner run on the NuGet packages, with results in Security →
  Code scanning.

## Build and verify

```
dotnet run --project tools/FontPack -- fetch
dotnet build -c Release
dotnet test -c Release
```

The build is warning-free (warnings are errors). After changing a font, a script list or the fallback order, render a
page of every affected script and compare it with Chrome printing the same HTML with the same fonts installed.

## Commit guidance

- Branch from the latest `origin/main`, one branch per piece of work, named after the work in lowercase words joined
  by hyphens, with a `claude/` prefix when an agent creates it. Never use a generic or generated name, and never reuse
  a branch name after its pull request is merged or closed.
- Commit subjects are short imperative or descriptive sentences without prefixes; the body explains why.
- Fill in `.github/pull_request_template.md`, and update the README of each package a change touches.
- Releases are the owner's step (`RELEASING.md`).

## Wisdom

- The Noto CJK collections hold five regional families (and five monospaced ones) that share most glyphs; regional
  forms differ for some characters, so the language passed to `AddNotoSansCjk` decides which region fallback uses.
- CJK files compress only about 20% with Brotli, so they are embedded raw: no decompression on first use, and the
  .nupkg's zip compression covers the download.
- A PDF with Noto CJK carries about 200 KB per face it uses: a CID-keyed CFF subset keeps every glyph id, so the
  CharStrings offsets and hmtx of 65,000 glyphs stay. Renumbering glyphs in the library would shrink that.
- PdfPecker has no CBDT (bitmap colour glyph) support, so the emoji package uses the COLRv1 build of Noto Color Emoji.
