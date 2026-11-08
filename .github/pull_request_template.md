## What changes

<!-- What a user of the packages sees before and after this change. -->

## How

<!-- A short note on the approach, if it isn't obvious from the diff. -->

## Checklist

- [ ] `dotnet build -c Release` and `dotnet test -c Release` pass
- [ ] `dotnet run --project tools/FontPack -- check` passes (generated lists match `sources.json`)
- [ ] A new or changed font or pattern file is pinned in `sources.json` by commit URL and SHA-256, and listed in `THIRD-PARTY-NOTICES.md`
- [ ] No new run-time dependency besides PdfPecker, and no reflection
- [ ] New public members have XML doc comments; the README of each changed package is updated
