using System;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Skybrud.Essentials.Reflection.Extensions;
using Umbraco.Cms.Core.Configuration.Models;
using Umbraco.Cms.Core.DependencyInjection;

namespace Limbo.Umbraco.YouTube.Extensions;

/// <summary>
/// Static class with various extensions methods for use in this package and using this package.
/// </summary>
public static class YouTubeExtensions {

    internal static IUmbracoBuilder AddUmbracoOptions<TOptions>(this IUmbracoBuilder builder, Action<OptionsBuilder<TOptions>>? configure = null) where TOptions : class {

        UmbracoOptionsAttribute umbracoOptionsAttribute = typeof(TOptions)
            .GetCustomAttribute<UmbracoOptionsAttribute>() ?? throw new ArgumentException($"{typeof(TOptions)} do not have the UmbracoOptionsAttribute.");

        OptionsBuilder<TOptions> optionsBuilder = builder.Services.AddOptions<TOptions>().Bind(
            builder.Config.GetSection(umbracoOptionsAttribute.ConfigurationKey),
            o => o.BindNonPublicProperties = umbracoOptionsAttribute.BindNonPublicProperties
        )
        .ValidateDataAnnotations();

        configure?.Invoke(optionsBuilder);

        return builder;

    }

    /// <summary>
    /// Returns whether the YouTube package has the necessary configuration in <c>appsettings.json</c> or similar.
    /// </summary>
    /// <param name="configuration">The configuration.</param>
    /// <returns><see langword="true"/> if at least one set of credentials has been added; otherwise, <see langword="false"/>.</returns>
    public static bool IsYouTubeConfigured(this IConfiguration configuration) {
        return configuration.GetSection("Limbo:YouTube:Credentials").GetChildren().Any();
    }

}