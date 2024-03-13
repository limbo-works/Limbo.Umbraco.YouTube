using Limbo.Umbraco.Video.Models.Providers;

namespace Limbo.Umbraco.YouTube.Models.Videos;

/// <summary>
/// Class with limited information about a video provider.
/// </summary>
public class YouTubeVideoProvider : VideoProvider {

    /// <summary>
    /// Gets a reference to a <see cref="YouTubeVideoProvider"/> instance.
    /// </summary>
    public static readonly YouTubeVideoProvider Default = new();

    private YouTubeVideoProvider() : base("youtube", "YouTube") { }

}