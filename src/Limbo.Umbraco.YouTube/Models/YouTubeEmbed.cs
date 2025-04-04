using System.Globalization;
using Limbo.Umbraco.Video.Models.Videos;
using Limbo.Umbraco.YouTube.Options;
using Limbo.Umbraco.YouTube.PropertyEditors;
using Microsoft.AspNetCore.Html;
using Newtonsoft.Json;
using Skybrud.Essentials.Json.Newtonsoft.Converters;

namespace Limbo.Umbraco.YouTube.Models;

/// <summary>
/// Class representing the embed options of the video.
/// </summary>
public class YouTubeEmbed : IVideoEmbed {

    #region Properties

    /// <summary>
    /// Gets the embed URL.
    /// </summary>
    [JsonProperty("url")]
    public string Url { get; }

    /// <summary>
    /// Gets whether embedded videos should automatically start playing.
    /// </summary>
    [JsonProperty("autoplay", NullValueHandling = NullValueHandling.Ignore)]
    public bool? Autoplay { get; }

    /// <summary>
    /// Gets whether embedded videos should loop.
    /// </summary>
    [JsonProperty("loop", NullValueHandling = NullValueHandling.Ignore)]
    public bool? Loop { get; }

    /// <summary>
    /// Gets whether the player controls should be visible.
    /// </summary>
    [JsonProperty("controls", NullValueHandling = NullValueHandling.Ignore)]
    public bool? ShowControls { get; }

    /// <summary>
    /// Gets whether the player should show related videos once the video has finished playing.
    /// </summary>
    [JsonProperty("rel", NullValueHandling = NullValueHandling.Ignore)]
    public bool? ShowRelated { get; }

    /// <summary>
    /// Gets whether the embed code should use a cookieless player. Notice that this doesn't entirely disable cookies,
    /// but the player won't set any cookies until the user starts the video.
    /// </summary>
    [JsonProperty("cookieless", NullValueHandling = NullValueHandling.Ignore)]
    public bool? DisableCookies { get; }

    /// <summary>
    /// Gets the time of the video that the player should start playing at, if any.
    /// </summary>
    [JsonProperty("start", NullValueHandling = NullValueHandling.Ignore)]
    public string? Start { get; }

    /// <summary>
    /// Gets the time of the video that the player should stop at, if any.
    /// </summary>
    [JsonProperty("end", NullValueHandling = NullValueHandling.Ignore)]
    public string? End { get; }

    /// <summary>
    /// Gets the HTML embed code.
    /// </summary>
    [JsonProperty("html")]
    [JsonConverter(typeof(StringJsonConverter))]
    public IHtmlContent Html { get; }

    /// <summary>
    /// Gets the property editor configuration, if any.
    /// </summary>
    [JsonIgnore]
    public YouTubeVideoConfiguration? Config { get; }

    #endregion

    #region Constructors

    internal YouTubeEmbed(YouTubeVideoDetails video, YouTubeVideoParameters parameters, YouTubeVideoConfiguration? config) {

        Config = config;

        Autoplay = config?.Autoplay ?? parameters.Autoplay;
        Loop = config?.Loop ?? parameters.Loop;
        ShowControls = config?.ShowControls ?? parameters.ShowControls;
        ShowRelated = config?.ShowRelated ?? parameters.ShowRelated;
        DisableCookies = config?.DisableCookies ?? parameters.DisableCookies;
        Start = parameters.Start?.TotalSeconds.ToString(CultureInfo.InvariantCulture);
        End = parameters.End?.TotalSeconds.ToString(CultureInfo.InvariantCulture);

        // Initialize the player options
        YouTubeEmbedPlayerOptions player = new() {
            Autoplay = config?.Autoplay ?? parameters.Autoplay,
            Loop = config?.Loop ?? parameters.Loop,
            ShowControls = config?.ShowControls ?? parameters.ShowControls,
            ShowRelated = config?.ShowRelated ?? parameters.ShowRelated,
            DisableCookies = config?.DisableCookies ?? parameters.DisableCookies,
            Start = parameters.Start,
            End = parameters.End
        };

        // Initialize the embed options
        YouTubeEmbedOptions o = new(video, player);

        Url = o.GetEmbedUrl();
        Html = o.GetEmbedCode();

    }

    #endregion

}