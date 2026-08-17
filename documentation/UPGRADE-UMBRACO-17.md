# Upgrade recap: Umbraco 13 → Umbraco 17

This document records what changed when `Limbo.Umbraco.YouTube` was ported from Umbraco 13 (`v13/main`) to
Umbraco 17 on the `v17/dev` branch, and why. The released version of this branch is **17.0.0-alpha000**.

The port follows the conventions already established by the sibling package
[`Limbo.Umbraco.TwentyThree` 17.0.0-alpha001](https://github.com/limbo-works/Limbo.Umbraco.TwentyThree/tree/v17/dev),
so the two packages stay consistent.

---

## 1. Headline changes

| Area | Umbraco 13 | Umbraco 17 |
|------|------------|------------|
| Target framework | `net8.0` | `net10.0` |
| Backoffice UI | AngularJS controller + HTML view | Lit web components (`custom elements`) |
| Backoffice registration | `IManifestFilter` (C#) | `IPackageManifestReader` + `backofficeEntryPoint` (JS) |
| HTTP API | `UmbracoAuthorizedApiController` + `[PluginController]` | Management API (`ManagementApiControllerBase`) |
| API route | `/umbraco/backoffice/Limbo/YouTube/GetVideo` | `/umbraco/management/api/v1/limbo/youtube/video` |
| Data type config UI | `.html` views referenced from `[ConfigurationField]` | `propertyEditorSchema.meta.settings` in JS |
| Asset pipeline | LESS compiled by Web Compiler | none — plain CSS inside the components |

Deliberately **not** changed: the JSON shape stored in the database, the public C# model API
(`YouTubeVideoValue`, `YouTubeVideoDetails`, `YouTubeEmbed`, …), and the property editor alias
`Limbo.Umbraco.YouTube`. Existing content and Razor views keep working.

---

## 2. Project file

`src/Limbo.Umbraco.YouTube/Limbo.Umbraco.YouTube.csproj`

- `TargetFramework` → `net10.0`. `LangVersion` was dropped; `net10.0` already defaults to C# 14.
- `VersionPrefix` → `17.0.0-alpha000`.
- Umbraco package references pinned to **`[17.0.0,17.9.9)`**:
  - `Umbraco.Cms.Core`
  - `Umbraco.Cms.Web.Website`
  - `Umbraco.Cms.Api.Management` *(new — needed for the Management API controller base class)*
  - `Umbraco.Cms.Web.Common` *(new — replaces `Umbraco.Cms.Web.BackOffice`, which no longer exists)*
- `Umbraco.Cms.Web.BackOffice` **removed** — the AngularJS backoffice assembly is gone in Umbraco 17.
- `Limbo.Umbraco.Video` → `17.0.0-alpha001`.
- `Skybrud.Social.Google.YouTube` → `1.0.2`.
- Added `Skybrud.Essentials` `1.1.68` and `Skybrud.Essentials.AspNetCore` `1.0.2` as direct references. The
  latter supplies `NewtonsoftJsonResult`, which the controller needs (see §4).
- Added `<NuGetAuditMode>direct</NuGetAuditMode>` so vulnerability warnings from transitive Umbraco
  dependencies don't fail the build.
- Removed the `compilerconfig.json` / `Intellisense.js` packaging exclusions — those files are gone.

Also deleted `src/build/Limbo.Umbraco.YouTube.targets`. It copied files out of a `content/App_Plugins` folder
that this package never shipped; static web assets (`StaticWebAssetBasePath`) handle asset delivery, and NuGet
now generates the `build/*.props` files automatically.

---

## 3. Backoffice manifest

**Deleted** `Manifests/YouTubeManifestFilter.cs`, **added** `Manifests/YouTubePackageManifestReader.cs`.

`IManifestFilter` and `builder.ManifestFilters()` were removed in Umbraco 14. A package now implements
`Umbraco.Cms.Infrastructure.Manifest.IPackageManifestReader` and returns a `PackageManifest` that:

1. registers a single extension of type `backofficeEntryPoint`, pointing at `EntryPoint.js`; and
2. declares an **importmap** so the package's own modules get stable bare specifiers:

   | Specifier | File |
   |-----------|------|
   | `@limbo/youtube/auth` | `Auth.js` |
   | `@limbo/youtube/package` | `Package.js` |
   | `@limbo/youtube/service` | `Service.js` |

Every URL carries `?v=<md5 of the informational version>` as a cache buster, replacing the old
`View += "?v=" + InformationalVersion` trick in the property editor.

Registered in `Composers/YouTubeComposer.cs` with
`builder.Services.AddSingleton<IPackageManifestReader, YouTubePackageManifestReader>()`.

---

## 4. HTTP API

**Rewritten** `Controllers/YouTubeController.cs`; **added** the `Api/` folder.

`UmbracoAuthorizedApiController` and `[PluginController]` no longer exist. The controller is now a versioned
Management API controller:

```csharp
[ApiController]
[VersionedApiBackOfficeRoute(YouTubeApiConstants.Route)]   // limbo/youtube
[Authorize(Policy = AuthorizationPolicies.SectionAccessContent)]
[MapToApi(YouTubeApiConstants.Alias)]                      // limbo-youtube-v1
[ApiVersion("1.0")]
[ApiExplorerSettings(GroupName = YouTubeApiConstants.GroupName)]
public class YouTubeController : ManagementApiControllerBase { … }
```

Supporting files, all new:

- `Api/YouTubeApiConstants.cs` — route, Swagger document alias, display names.
- `Api/YouTubeSwaggerGenOptions.cs` — registers a dedicated Swagger document for this package, so its
  endpoints don't pollute Umbraco's own document. Wired up with `builder.Services.ConfigureOptions<…>()`.
- `Api/YouTubeSecurityFilter.cs` — derives from `BackOfficeSecurityRequirementsOperationFilterBase` so the
  Swagger UI knows the endpoints need backoffice authentication.

### Endpoint changes

| Old | New |
|-----|-----|
| `GET/POST /umbraco/backoffice/Limbo/YouTube/GetVideo` (`source` in query or form) | `GET /umbraco/management/api/v1/limbo/youtube/video?source=…` |
| — | `GET /umbraco/management/api/v1/limbo/youtube/serverVariables` |

`serverVariables` is new. The old client read the version and cache buster from
`Umbraco.Sys.ServerVariables`, which the new backoffice does not expose to packages, so the values are
fetched from this endpoint at startup instead.

The `POST` overload was dropped — the client only ever issued one lookup call, and a `GET` with a query
string is the natural Management API shape.

### Why `NewtonsoftJsonResult`

The Management API serializes with **System.Text.Json**, but the intermediary models
(`YouTubeIntermediaryVideoValue` and friends) are annotated with Newtonsoft's `[JsonProperty]`, and
`YouTubeJsonConverter` is a Newtonsoft `JsonConverter`. Serializing them with System.Text.Json would silently
produce a different shape than the one stored in the database. The successful response is therefore written
through `NewtonsoftJsonResult.Ok(...)` from `Skybrud.Essentials.AspNetCore`.

Error handling also changed slightly: failures that used to return `400 Bad Request` with the exception
message now return `500 Internal Server Error`, which is more accurate for YouTube API/configuration
failures. Invalid sources still return `400`, and unknown videos still return `404`.

---

## 5. Property editor (server side)

### `PropertyEditors/YouTubeVideoPropertyEditor.cs`

`[DataEditor]` no longer carries a name, view, icon or group — those are client-side concerns now:

```csharp
[DataEditor(EditorAlias, ValueType = ValueTypes.Json)]
```

The `GetValueEditor` override that appended a cache-busting query string to the view URL is gone (there is no
server-rendered view any more). `SupportsReadOnly = true` is now set in the constructor.

### `PropertyEditors/YouTubeVideoConfigurationEditor.cs`

- `ConfigurationEditor<T>` takes only `IIOHelper`; the `IEditorConfigurationParser` parameter was removed.
- The loop that rewrote `field.View` placeholders (`{version}`, `{alias}`) is gone.
- Added a `DefaultConfiguration` that defaults `cacheLevel` to `Element`.

### `PropertyEditors/YouTubeVideoConfiguration.cs`

- `[ConfigurationField]` now only takes the alias. Labels, descriptions and the editing UI for each field moved
  to the `propertyEditorSchema` settings in `EntryPoint.js`.
- **`CacheLevel` changed type from `PropertyCacheLevel?` to `string?`.** Data type configuration is
  deserialized by Umbraco's own configuration serializer, and rather than depend on how that serializer
  handles enums, the value is kept as a string and mapped in the value converter.
- **The `hideLabel` field was removed.** The new backoffice controls label visibility on the document type
  property, not on the data type, so the setting no longer had any effect.

### `PropertyEditors/YouTubeVideoValueConverter.cs`

- `propertyType.DataType.Configuration as YouTubeVideoConfiguration` →
  `propertyType.DataType.ConfigurationAs<YouTubeVideoConfiguration>()`.
- `PropertyCacheLevel.Elements` → `PropertyCacheLevel.Element` as the default. All five enum members
  (`Element`, `Elements`, `Snapshot`, `None`, `Unknown`) still exist in Umbraco 17; the configured string is
  parsed with `Enum.TryParse`, falling back to `Element`.
- The `details` / legacy `video` property fallback is unchanged.

---

## 6. Models

Only one breaking change came from `Limbo.Umbraco.Video` 17:

`IVideoDetails.Thumbnails` and `IVideoDetails.Files` are now `IReadOnlyList<>` instead of `IEnumerable<>`.
`Models/YouTubeVideoDetails.cs` was updated accordingly (including a `.ToArray()` when projecting the
thumbnails, and the explicit interface implementation).

Two deprecated Skybrud APIs were also swapped for their current equivalents, with no change in behaviour:

| Old | New |
|-----|-----|
| `Skybrud.Essentials.Json.Converters.Time.TimeSpanSecondsConverter` | `Skybrud.Essentials.Json.Newtonsoft.Converters.Time.TimeSpanConverter` (defaults to `TimeSpanFormat.Seconds`, so `duration` still serializes as seconds) |
| `using Skybrud.Essentials.Json.Extensions` | `using Skybrud.Essentials.Json.Newtonsoft.Extensions` |
| `using Skybrud.Essentials.Collections.Extensions` | `using Skybrud.Essentials.Collections.Lists.Extensions` |

`YouTubePackage` became a `static class`, and `SemVersion` is now parsed from the already-trimmed
`InformationalVersion` (the old code parsed the raw value, which includes a `+<commit>` suffix).
`DocumentationUrl` points at the `v17` docs.

`Services/YouTubeService.cs`, the `Exceptions/`, `Options/` and `Json/` folders and the intermediary models are
otherwise untouched — the URL/embed-code parsing and the YouTube API calls did not need to change.

---

## 7. Backoffice client (complete rewrite)

### Removed

```
wwwroot/Scripts/Controllers/{Video,ButtonList,CacheLevel}.js   AngularJS controllers
wwwroot/Scripts/Services/YouTubeService.js                     AngularJS $http service
wwwroot/Scripts/Intellisense.js
wwwroot/Views/{Video,ButtonList,CacheLevel}.html               AngularJS templates
wwwroot/Styles/Default.{less,css}
wwwroot/Lang/{en-US,da-DK}.xml                                 server-side translations
wwwroot/BackOffice/Icons/*.svg
compilerconfig.json, compilerconfig.json.defaults              Web Compiler (LESS) config
```

### Added

| File | Purpose |
|------|---------|
| `wwwroot/EntryPoint.js` | The `backofficeEntryPoint`. Waits for `UMB_AUTH_CONTEXT`, stores the auth token, fetches the server variables, then registers every extension. |
| `wwwroot/Package.js` | Holds the server variables (version, cache buster). |
| `wwwroot/Auth.js` | Holds the Management API token getter. |
| `wwwroot/Service.js` | `fetch`-based API client (replaces the AngularJS `$http` service) plus a `getThumbnail` helper. |
| `wwwroot/Icons.js` + `wwwroot/Icons/YouTube.js` | Icon registration. The SVG is now a JS template string using `fill="currentColor"` so it follows the backoffice theme. Both `limbo-youtube` and `limbo-youtube-alt` are registered, as before. |
| `wwwroot/Elements/Video.js` | The property editor UI, `<limbo-youtube-video>`. |
| `wwwroot/Elements/ButtonList.js` | `<limbo-youtube-button-list>`, used for the data type configuration fields. |
| `wwwroot/Localization/{en-US,da-DK}.js` | Client-side translations under the `limboYouTube` area. |

### Extensions registered by `EntryPoint.js`

| Type | Alias |
|------|-------|
| `localization` | `Limbo.Umbraco.YouTube.EnUs`, `Limbo.Umbraco.YouTube.DaDk` |
| `icons` | `Limbo.Umbraco.YouTube.Icons` |
| `propertyEditorSchema` | `Limbo.Umbraco.YouTube` *(matches the C# `EditorAlias`)* |
| `propertyEditorUi` | `Limbo.Umbraco.YouTube.Video.Ui` |
| `propertyEditorUi` | `Limbo.Umbraco.YouTube.ButtonList.PropertyEditorUi` |

The `propertyEditorSchema` alias intentionally equals the existing `[DataEditor]` alias, so data types created
by earlier versions of the package resolve without migration.

### Data type configuration UI

The six configuration fields are declared as `propertyEditorSchema.meta.settings.properties`, all rendered by
`ButtonList`:

- `cacheLevel` — `Element` / `Elements` / `Snapshot` / `None` (defaults to `Element`)
- `autoplay`, `loop`, `controls`, `rel`, `disableCookies` — tri-state *Not specified* / *Yes* / *No*

`ButtonList` matches items on their **`value`** rather than an alias, so it can store `null`, `true`/`false`
and strings. That is what lets the tri-state options keep binding to the `bool?` properties on
`YouTubeVideoConfiguration`, where `null` still means "not specified, fall back to the video URL".

### `Elements/Video.js` vs. the old AngularJS controller

- Extends `UmbFormControlMixin(UmbLitElement)` and registers the `<textarea>` as its form control element, so
  mandatory/validation states work.
- **The API response is stored as the property value verbatim.** The `video` response already has
  exactly the shape the value converter expects (`source`, `credentials`, `parameters`, `details._data`), so the
  old field-by-field copying — and the `value` → `details` rename shim on save — are gone. The reader still
  accepts the pre-13.0.2 `video` key when parsing an existing value.
- Lookups are **debounced by 300 ms** while typing, and a request token discards the result of a lookup that
  has been superseded. The old controller fired a request on every change event.
- Emits `UmbChangeEvent` instead of writing to `$scope.model.value`. The empty value is `null`; the old
  workaround of saving an empty string is not needed.
- The `_data` indirection is unchanged: the raw YouTube response is still nested as an escaped JSON string,
  because Umbraco would otherwise mangle the timestamps in it.
- The duration is rendered with `<limbo-video-duration>` from `Limbo.Umbraco.Video`, imported via that
  package's own importmap entry `@limbo/video/elements/duration` (previously the AngularJS
  `<limbo-video-duration>` directive).
- The two debug `<pre>{{model.value | json}}</pre>` blocks that shipped in the v13 view were dropped.

---

## 8. Verification performed

- `dotnet build -c Release` — succeeds with **no errors and no warnings**.
- `dotnet build -c Release /t:rebuild /t:pack` — produces
  `releases/nuget/Limbo.Umbraco.YouTube.17.0.0-alpha000.nupkg` containing `lib/net10.0/` and all ten
  `staticwebassets/` files, with the dependency ranges above.
- The package was installed into a freshly generated Umbraco 17.5.0 site (`dotnet new umbraco`, SQLite,
  unattended install) and reported compatible for all target frameworks.
- That site was booted (`ASPNETCORE_URLS=http://localhost:5599 dotnet run`) and **started with no errors or
  failures in the log**, confirming the composer, manifest reader and Swagger registration all load.
- `YouTubePackageManifestReader` was exercised in that running site and produces the expected manifest:
  id `Limbo.Umbraco.YouTube`, version `17.0.0-alpha000`, one `backofficeEntryPoint` extension and the three
  `@limbo/youtube/*` importmap entries, each with a `?v=<md5>` cache buster.
- The `Limbo.Umbraco.Video` manifest in the same site confirms the `@limbo/video/elements/duration` specifier
  that `Elements/Video.js` imports really is provided.
- Every bare specifier imported by the new client code resolves against an importmap entry present at runtime:
  `@umbraco-cms/backoffice/{auth,element-api,event,external/lit,lit-element,validation}`,
  `@limbo/youtube/{auth,package,service}` and `@limbo/video/elements/duration`.
- All ten static assets are served over HTTP (`200`) from `/App_Plugins/Limbo.Umbraco.YouTube/…`.
- All ten JS modules pass `node --check`.
- Both Management API endpoints route correctly and enforce backoffice authentication — an unauthenticated
  request to `/video` and `/serverVariables` returns `401`, not `404`.
- The package's Swagger document is generated at `/umbraco/swagger/limbo-youtube-v1/swagger.json`, with the
  title `Limbo YouTube API v1`, both endpoints, and `security: [{ "Backoffice-User": [] }]` on each operation
  (see §10 for how that was proven).

### Not verified

- **Live video lookups against the YouTube Data API.** This needs a real Google API key
  (`Limbo:YouTube:Credentials[0].ApiKey`), so the request/response path through `YouTubeService` and the
  YouTube API was not exercised end to end.
- **Interactive browser testing** of the property editor (pasting a URL, rendering the video card, saving and
  reloading a document) — worth doing on a site that has an API key before promoting this past `alpha`.

---

## 9. Follow-ups

- Publish `Limbo.Umbraco.Video` and this package past `alpha`; both currently depend on prerelease builds.
- Consider whether `parameters` should still be parsed from the URL query string client-side, now that the
  data type configuration always wins in `YouTubeEmbed`.
- The `README.md` install snippets and the docs site (`packages.limbo.works/limbo.umbraco.youtube/v17/`) need
  the v17 content.

---

## 10. Post-review fixes

A code review after the initial port found three defects, all since fixed. They are recorded here because two
of them were inherited by copying `Limbo.Umbraco.TwentyThree`, and are therefore probably still present there.

### 10.1 `YouTubeSecurityFilter.ApiName` returned the wrong value

`Api/YouTubeSecurityFilter.cs` originally returned `YouTubeApiConstants.Name`
(`"Limbo YouTube API v1"` — the Swagger document *title*). `BackOfficeSecurityRequirementsOperationFilterBase`
matches `ApiName` against the Swagger **document name**, which is the value passed to `options.SwaggerDoc(...)`
and to `[MapToApi(...)]` — i.e. `YouTubeApiConstants.Alias` (`"limbo-youtube-v1"`). With the wrong value the
filter silently never fired.

This was confirmed empirically rather than by reading Umbraco's source, by diffing the generated Swagger
document between the two values in a running Umbraco 17.5.0 site:

| `ApiName` | `security` on our operations |
|-----------|------------------------------|
| `Name` (original) | absent |
| `Alias` (fixed) | `[{ "Backoffice-User": [] }]` |

For reference, 448 of Umbraco's own 458 management-API operations carry the same per-operation `security`
block, which is what a correctly-registered filter looks like.

**Impact was limited to the Swagger/OpenAPI description, not to runtime security** — access is enforced by the
`[Authorize(Policy = AuthorizationPolicies.SectionAccessContent)]` attribute on the controller, which was
always correct and is what produces the `401` responses noted in §8. The bug meant the Swagger UI didn't know
to send a bearer token, so the endpoints appeared callable anonymously there and would have returned `401`.

`Limbo.Umbraco.TwentyThree` 17.0.0-alpha001 has the same bug (`ApiName => TwentyThreeApiConstants.Name`).

### 10.2 `Elements/Video.js` accepted stale lookup results while typing

`#scheduleLookup` cleared the debounce timer but did not advance `#requestToken`, so a request that was
*already in flight* when the user typed again still passed the `requestId !== this.#requestToken` guard in
`#lookup` and overwrote the newer input. `#scheduleLookup` now increments the token, invalidating any in-flight
lookup, and `#onRefresh` clears the debounce timer so refreshing cannot fire a second identical request.

Separately, `#scheduleLookup` used to set `#loading = true` for the whole debounce window. Since
`.loading > div` applies `pointer-events: none`, that made the entire editor — including the textarea being
typed into — unclickable while typing. The loading state is now only set once the request actually starts.

### 10.3 An inaccurate code comment

`YouTubeVideoValueConverter` carried a comment claiming `PropertyCacheLevel.Elements` had been *renamed* to
`PropertyCacheLevel.Element` in Umbraco 17. It wasn't — all five members still exist (as §5 already stated
correctly). What changed is the **fallback** used when a data type has no explicit `cacheLevel`, which went
from `Elements` (v13) to `Element`. That is a deliberate behaviour change: existing data types that never had
the setting configured will cache at a narrower scope than before. Set `cacheLevel` explicitly on the data type
to keep the old behaviour.

### Re-verification after the fixes

`dotnet build -c Release` still succeeds with no errors or warnings, all ten JS modules still pass
`node --check`, and `releases/nuget/Limbo.Umbraco.YouTube.17.0.0-alpha000.nupkg` was repacked so the shipped
package contains the fixes. The Swagger check in §8 was run against the fixed code.
