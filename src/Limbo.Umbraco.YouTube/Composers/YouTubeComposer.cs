using Limbo.Umbraco.YouTube.Extensions;
using Limbo.Umbraco.YouTube.Manifests;
using Limbo.Umbraco.YouTube.Models.Settings;
using Limbo.Umbraco.YouTube.Services;
using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;

#pragma warning disable 1591

namespace Limbo.Umbraco.YouTube.Composers;

public class YouTubeComposer : IComposer {

    public void Compose(IUmbracoBuilder builder) {

        builder.Services.AddTransient<YouTubeService>();

        builder.ManifestFilters().Append<YouTubeManifestFilter>();

        builder.AddUmbracoOptions<YouTubeSettings>();

    }

}