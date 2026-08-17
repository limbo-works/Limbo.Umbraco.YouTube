using Limbo.Umbraco.YouTube.Api;
using Limbo.Umbraco.YouTube.Extensions;
using Limbo.Umbraco.YouTube.Manifests;
using Limbo.Umbraco.YouTube.Models.Settings;
using Limbo.Umbraco.YouTube.Services;
using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Infrastructure.Manifest;

#pragma warning disable 1591

namespace Limbo.Umbraco.YouTube.Composers;

public class YouTubeComposer : IComposer {

    public void Compose(IUmbracoBuilder builder) {

        builder.Services.AddTransient<YouTubeService>();

        builder.AddUmbracoOptions<YouTubeSettings>();

        // [CHANGE: Umbraco 17 upgrade - ManifestFilters() is gone; the backoffice manifest is now supplied
        // by an IPackageManifestReader, and the package's API needs its own Swagger document]
        // Related: see documentation/UPGRADE-UMBRACO-17.md for the full list of changed files.
        builder.Services.AddSingleton<IPackageManifestReader, YouTubePackageManifestReader>();

        builder.Services.ConfigureOptions<YouTubeSwaggerGenOptions>();

    }

}
