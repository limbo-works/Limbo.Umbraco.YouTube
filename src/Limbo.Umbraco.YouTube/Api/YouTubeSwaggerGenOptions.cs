// [CHANGE: Umbraco 17 upgrade - registers a separate Swagger document for this package's API]
// Related: see documentation/UPGRADE-UMBRACO-17.md for the full list of changed files.

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Limbo.Umbraco.YouTube.Api;

public class YouTubeSwaggerGenOptions : IConfigureOptions<SwaggerGenOptions> {

    public void Configure(SwaggerGenOptions options) {
        options.SwaggerDoc(YouTubeApiConstants.Alias, new OpenApiInfo {
            Title = YouTubeApiConstants.Name,
            Version = "1.0"
        });
        options.OperationFilter<YouTubeSecurityFilter>();
    }

}
