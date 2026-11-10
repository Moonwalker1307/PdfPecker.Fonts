# Releasing

The packages always have the version of the PdfPecker release they are built on, and depend on it as a minimum. A
release here follows each PdfPecker release that the packages should move to; nothing here is released on its own.

1. Wait until nuget.org lists the PdfPecker version (its flat container can lag the search page by some minutes).
2. In a pull request, set `PdfPeckerVersion` in `Directory.Build.props` to that version. If the release added
   something the packages use, such as a font or hyphenation API, add it in the same pull request. CI then builds
   against the published library instead of the library's main branch.
3. After that pull request merges, the owner publishes a GitHub release with the tag `v<version>`. The Build workflow
   builds from a fresh download of every pinned file, refuses a tag that differs from `PdfPeckerVersion` or a library
   version that nuget.org does not have, and pushes all five packages with NuGet Trusted Publishing.

Only the repository owner publishes a release. Trusted Publishing needs, once: a policy on nuget.org for this
repository, workflow `build.yml` and environment `nuget`; the GitHub environment `nuget`; and the repository variable
`NUGET_USER` set to the nuget.org account.

## Building locally

`nuget.config` takes the PdfPecker package from `local-feed/` alone, so a build needs it there, also once nuget.org
has the version (CI downloads it the same way):

```
mkdir -p local-feed
curl -fsSL -o local-feed/pdfpecker.<version>.nupkg https://api.nuget.org/v3-flatcontainer/pdfpecker/<version>/pdfpecker.<version>.nupkg
```

## Between library changes and their release

When the packages need a library change that is merged but not released, `PdfPeckerVersion` names the coming version
and CI packs the library's main branch with that version into `local-feed/`, with a warning. Locally, do the same:

```
dotnet pack ../PdfPecker/src/PdfPecker -c Release -p:Version=<version> -p:EnablePackageValidation=false -o local-feed
```

and clear `~/.nuget/packages/pdfpecker/<version>` afterwards, so the published package replaces it.

## Updating a font or pattern file

Change the file's URL (a commit, never a branch) and SHA-256 in `sources.json`, run
`dotnet run --project tools/FontPack -- fetch` and `-- generate`, update `THIRD-PARTY-NOTICES.md`, and look at the
PDFs of every script the file covers.
