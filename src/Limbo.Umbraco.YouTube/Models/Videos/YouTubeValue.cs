using System.Diagnostics.CodeAnalysis;
using Limbo.Umbraco.Video.Models.Providers;
using Limbo.Umbraco.Video.Models.Videos;
using Limbo.Umbraco.YouTube.PropertyEditors;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Skybrud.Essentials.Json.Extensions;

namespace Limbo.Umbraco.YouTube.Models.Videos;

/// <summary>
/// Class representing the value of the <see cref="YouTubeEditor"/> property editor.
/// </summary>
public class YouTubeValue : IVideoValue {

    #region Properties

    /// <summary>
    /// Gets the source (URL or embed code) as entered by the user.
    /// </summary>
    [JsonIgnore]
    public string Source { get; }

    /// <summary>
    /// Gets information about the video provider.
    /// </summary>
    [JsonProperty("provider")]
    public YouTubeVideoProvider Provider { get; }

    /// <summary>
    /// Gets the embed parameters specified for the video.
    /// </summary>
    [JsonIgnore]
    public YouTubeVideoParameters Parameters { get; }

    /// <summary>
    /// Gets the details about the picked video.
    /// </summary>
    [JsonProperty("details")]
    public YouTubeVideoDetails Details { get; }

    /// <summary>
    /// Gets embed information for the video.
    /// </summary>
    [JsonProperty("embed")]
    public YouTubeEmbed Embed { get; }

    IVideoProvider IVideoValue.Provider => Provider;

    IVideoDetails IVideoValue.Details => Details;

    IVideoEmbed IVideoValue.Embed => Embed;

    #endregion

    #region Constructors

    private YouTubeValue(JObject json, YouTubeConfiguration? config) {
        Source = json.GetString("source")!;
        Provider = YouTubeVideoProvider.Default;
        Parameters = YouTubeVideoParameters.Parse(json.GetObject("parameters") ?? new JObject());
        Details = json.GetObject("video", YouTubeVideoDetails.Parse)!;
        Embed = new YouTubeEmbed(Details, Parameters, config);
    }

    #endregion

    #region Static methods

    /// <summary>
    /// Parses the specified <paramref name="json"/> object into an instance of <see cref="YouTubeValue"/>.
    /// </summary>
    /// <param name="json">An instance of <see cref="JObject"/> representing the value.</param>
    /// <param name="config">The data type configuration.</param>
    /// <returns>An instance of <see cref="YouTubeValue"/> if <paramref name="json"/> is not null; otherwise, <see langword="null"/>.</returns>
    [return: NotNullIfNotNull("json")]
    public static YouTubeValue? Parse(JObject? json, YouTubeConfiguration? config) {
        return json == null ? null : new YouTubeValue(json, config);
    }

    #endregion

}