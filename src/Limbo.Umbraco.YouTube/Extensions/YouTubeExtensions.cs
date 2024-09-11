using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Skybrud.Essentials.Reflection.Extensions;
using Umbraco.Cms.Core.Configuration.Models;
using Umbraco.Cms.Core.DependencyInjection;

namespace Limbo.Umbraco.YouTube.Extensions;

internal static class YouTubeExtensions {

    internal static IUmbracoBuilder AddUmbracoOptions<TOptions>(this IUmbracoBuilder builder, Action<OptionsBuilder<TOptions>>? configure = null) where TOptions : class {

        UmbracoOptionsAttribute umbracoOptionsAttribute = typeof(TOptions)
            .GetCustomAttribute<UmbracoOptionsAttribute>() ?? throw new ArgumentException($"{typeof(TOptions)} do not have the UmbracoOptionsAttribute.");

        OptionsBuilder<TOptions>? optionsBuilder = builder.Services.AddOptions<TOptions>()
            .Bind(
                builder.Config.GetSection(umbracoOptionsAttribute.ConfigurationKey),
                o => o.BindNonPublicProperties = umbracoOptionsAttribute.BindNonPublicProperties
            )
            .ValidateDataAnnotations();

        configure?.Invoke(optionsBuilder);

        return builder;

    }

}