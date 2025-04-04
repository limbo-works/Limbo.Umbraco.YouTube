using Limbo.Umbraco.YouTube.Models.Videos.Intermediary;
using Limbo.Umbraco.YouTube.Services;
using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.DependencyInjection;

namespace Limbo.Umbraco.YouTube;

/// <summary>
/// Static class with various utility methods for YouTube implementation.
/// </summary>
public static class YouTubeUtils {

    /// <summary>
    /// Attempts to look up the video identified by the specified <paramref name="source"/>, and returns an instance of <see cref="YouTubeIntermediaryVideoValue"/> if successful. When serialized to JSON, the value equals the property value saved in the database for properties using the YouTube video data type.
    /// </summary>
    /// <param name="source">The source (URL or embed code) as entered by the user.</param>
    /// <returns>An instance of <see cref="YouTubeIntermediaryVideoValue"/> representing the video.</returns>
    public static YouTubeIntermediaryVideoValue GetIntermediaryVideoValue(string source) {
        return StaticServiceProvider.Instance
            .GetRequiredService<YouTubeService>()
            .GetIntermediaryVideoValue(source);
    }

}