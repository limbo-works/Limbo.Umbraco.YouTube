# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## What this is

`Limbo.Umbraco.YouTube` — a NuGet package (not an app) adding a **Limbo YouTube Video** property editor to Umbraco. Editors paste a YouTube URL or `<iframe>` embed code; the package resolves the video via the YouTube Data API and stores a JSON snapshot in the property value. Consumers read it as `YouTubeVideoValue` in C#/Razor.

Single project: `src/Limbo.Umbraco.YouTube/` (Razor SDK class library, .NET 10, nullable enabled). No test project.

## Commands

```bash
# Debug build + pack to a local NuGet feed (see debug.bat — path is Windows-specific)
dotnet build src/Limbo.Umbraco.YouTube --configuration Debug /t:rebuild /t:pack -p:PackageOutputPath=<local-feed>

# Release build + pack into ./releases/nuget (release.bat)
dotnet build src/Limbo.Umbraco.YouTube --configuration Release /t:rebuild /t:pack -p:PackageOutputPath=../../releases/nuget
```

Debug builds get a `build<yyyyMMddHHmm>` version suffix automatically. `src/Limbo.Umbraco.YouTube.sln` is the solution.

There is no client-side build step: `wwwroot/` ships as hand-written ES modules, served as static web assets under `App_Plugins/Limbo.Umbraco.YouTube` (`StaticWebAssetBasePath`). No npm, no bundler, no TypeScript, no LESS.

To smoke-test a change, pack into a local feed, `dotnet new umbraco` a throwaway site, add the package, and boot it. `documentation/UPGRADE-UMBRACO-17.md` §8 records what that verified last time.

## Branching / versioning

One release branch per Umbraco major: `v1/main` (U9), `v2/main` (U10-12), `v13/main` (U13, currently the repo's default branch), and `v17/dev` (U17, `17.0.0-alpha000`). Bump `VersionPrefix` in the `.csproj` for a release; the README install snippets embed the version number too.

**The sibling package [`Limbo.Umbraco.TwentyThree`](https://github.com/limbo-works/Limbo.Umbraco.TwentyThree) (branch `v17/dev`) is the reference implementation for this repo's v17 conventions** — same author, same shape (a video picker built on `Limbo.Umbraco.Video`). Check it before inventing a new pattern.

## Architecture

Data flow, backoffice → database → published content:

1. **Backoffice** (`wwwroot/Elements/Video.js`, a Lit element) posts the pasted source to `GET /umbraco/management/api/v1/limbo/youtube/video?source=…` via `wwwroot/Service.js`.
2. **`YouTubeController`** (`ManagementApiControllerBase`, `[VersionedApiBackOfficeRoute]`) delegates to `YouTubeService` and maps the package's exception types onto HTTP status codes.
3. **`YouTubeService`** is the only place that talks to YouTube. `TryGetVideoId` regex-extracts the video ID (unwrapping `<iframe src="…">` first) plus embed params from the query string into `YouTubeVideoOptions`; `GetIntermediaryVideoValue` calls the API (via `Skybrud.Social.Google.YouTube`, `snippet` + `contentDetails` parts) and returns a `YouTubeIntermediaryVideoValue`. It throws — it does not return null — so failures carry meaning (`YouTubeInvalidSourceException`, `YouTubeVideoNotFoundException`, `YouTubeNotConfiguredException`, `YouTubeServiceDisabledException`, …, all deriving from `YouTubeException`).
4. The serialized `YouTubeIntermediaryVideoValue` **is** the property value saved to the database — the client stores the API response verbatim. `YouTubeUtils.GetIntermediaryVideoValue` exposes the same thing statically for migrations/imports.
5. **`YouTubeVideoValueConverter`** parses that JSON back into `YouTubeVideoValue` (with `Details`, `Parameters`, `Embed`) for published content.

### Two model families — keep them straight

- `Models/Videos/Intermediary/*` — *write* models. Shape of the JSON stored in the database. Serialization-only.
- `Models/*` (`YouTubeVideoValue`, `YouTubeVideoDetails`, `YouTubeEmbed`, `YouTubeVideoParameters`) — *read* models built by the value converter, implementing `IVideoValue`/`IVideoDetails`/`IVideoEmbed` from the sibling `Limbo.Umbraco.Video` package. That interface contract is why `YouTubeVideoProvider`, empty `Files`, and the explicit interface members exist.

### Newtonsoft vs System.Text.Json

The package's models are annotated with **Newtonsoft** attributes, but Umbraco 17's Management API serializes with System.Text.Json. The controller therefore returns successful payloads through `NewtonsoftJsonResult.Ok(...)` (from `Skybrud.Essentials.AspNetCore`). Serializing the intermediary models with STJ would silently produce a different shape than the value converter expects — don't "simplify" this away.

### The `_data` indirection

Umbraco/JSON.NET mangles timestamps when round-tripping property JSON, so the raw YouTube API response is stored as an **escaped JSON string** under `details._data` (`YouTubeJsonConverter` writes it; `YouTubeVideoDetails.Parse` re-parses it; `Video.js` does `JSON.parse(details._data)`). Do not "simplify" this into a nested object.

### Legacy shape

`details` was called `video` before 13.0.2. Both C# (`json.GetObject("details") ?? json.GetObject("video")`) and JS keep reading the old key. Preserve those fallbacks.

### Embed options precedence

Data type configuration wins over per-video URL parameters: `config?.X ?? parameters.X` throughout `YouTubeEmbed`. `YouTubeEmbedOptions.GetEmbedCode()` builds the iframe (defaults 560×315). Nullable `bool?` is meaningful everywhere here — `null` = "not specified", and unspecified embed properties are deliberately omitted from the JSON via `NullValueHandling.Ignore`. The `ButtonList` element preserves this by matching config items on their `value` (which may be `null`), not on an alias.

### Registration — split across C# and JS

`YouTubeComposer` registers `YouTubeService`, binds `YouTubeSettings` (`Limbo:YouTube`, requires at least one credential set with an `ApiKey` — see README), registers `YouTubePackageManifestReader` as an `IPackageManifestReader`, and configures the package's own Swagger document (`Api/YouTubeSwaggerGenOptions`).

`YouTubePackageManifestReader` declares only two things: a `backofficeEntryPoint` pointing at `wwwroot/EntryPoint.js`, and an **importmap** mapping `@limbo/youtube/{auth,package,service}` to the corresponding files. Everything else — the `propertyEditorSchema`, both `propertyEditorUi`s, icons and localization — is registered at runtime in `EntryPoint.js`. **New backoffice module → add it to `EntryPoint.js`; new bare import specifier → also add it to the manifest reader's importmap.**

The `propertyEditorSchema` alias in `EntryPoint.js` must stay equal to `YouTubeVideoPropertyEditor.EditorAlias` (`Limbo.Umbraco.YouTube`), or existing data types stop resolving.

Data type configuration fields carry only an alias in C# (`[ConfigurationField("cacheLevel")]`); their labels, descriptions and editing UI live in `propertyEditorSchema.meta.settings` in `EntryPoint.js`. Both sides must agree on the alias.

All asset URLs carry `?v=<cacheBuster>`, where the cache buster comes from the `serverVariables` endpoint at startup.

## Conventions

`src/.editorconfig` governs formatting — 4 spaces, **CRLF**, no final newline, K&R braces (`{` on the same line), `#region` blocks (`Properties` / `Constructors` / `Member methods` / `Static methods`), file-scoped namespaces, BOM on `.cs` files. Public members need XML doc comments (`DocumentationFile` is on); files that intentionally skip them use `#pragma warning disable CS1591`. The JS follows the same brace/indent style and uses `#private` class fields. UI strings go in `wwwroot/Localization/{en-US,da-DK}.js` under the `limboYouTube` area, read via `this.localize.term("limboYouTube_<key>")`.

Builds are expected to be warning-free; several deprecated Skybrud.Essentials namespaces were fixed during the v17 port to keep it that way.
