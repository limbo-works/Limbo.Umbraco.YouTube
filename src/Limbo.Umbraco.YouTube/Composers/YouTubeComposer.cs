using System;
using Limbo.Umbraco.YouTube.Models;
using Limbo.Umbraco.YouTube.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Skybrud.Essentials.Reflection.Extensions;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.Configuration.Models;
using Umbraco.Cms.Core.DependencyInjection;

#pragma warning disable 1591

namespace Limbo.Umbraco.YouTube.Composers {
    
    public class YouTubeComposer : IComposer {

        public void Compose(IUmbracoBuilder builder) {
            
            builder.Services.AddTransient<YouTubeService>();
            
            builder.AddUmbracoOptions<YouTubeSettings>();

        }

    }

    internal static class YouTubeComposerExtensions {

        internal static IUmbracoBuilder AddUmbracoOptions<TOptions>(this IUmbracoBuilder builder, Action<OptionsBuilder<TOptions>> configure = null) where TOptions : class {

            var umbracoOptionsAttribute = typeof(TOptions).GetCustomAttribute<UmbracoOptionsAttribute>();
            if (umbracoOptionsAttribute is null) {
                throw new ArgumentException($"{typeof(TOptions)} do not have the UmbracoOptionsAttribute.");
            }

            var optionsBuilder = builder.Services.AddOptions<TOptions>()
                .Bind(
                    builder.Config.GetSection(umbracoOptionsAttribute.ConfigurationKey),
                    o => o.BindNonPublicProperties = umbracoOptionsAttribute.BindNonPublicProperties
                )
                .ValidateDataAnnotations();

            configure?.Invoke(optionsBuilder);

            return builder;

        }

    }

}