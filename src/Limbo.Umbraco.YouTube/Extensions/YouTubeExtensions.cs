using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Skybrud.Essentials.Http.Collections;
using Skybrud.Essentials.Reflection.Extensions;
using Skybrud.Essentials.Strings;
using Umbraco.Cms.Core.Configuration.Models;
using Umbraco.Cms.Core.DependencyInjection;

namespace Limbo.Umbraco.YouTube.Extensions {
    
    internal static class YouTubeExtensions {

        public static bool TryGetBoolean(this IHttpQueryString query, string key, out bool result) {
            // TODO: Move to Skybrud.Essentials.Http
            return StringUtils.TryParseBoolean(query[key], out result);
        }

        public static bool TryGetInt32(this IHttpQueryString query, string key, out int result) {
            // TODO: Move to Skybrud.Essentials.Http
            return int.TryParse(query[key], out result);
        }

        public static bool TryGetDouble(this IHttpQueryString query, string key, out double result) {
            // TODO: Move to Skybrud.Essentials.Http
            return double.TryParse(query[key], out result);
        }
        
        internal static IUmbracoBuilder AddUmbracoOptions<TOptions>(this IUmbracoBuilder builder, Action<OptionsBuilder<TOptions>>? configure = null) where TOptions : class {

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