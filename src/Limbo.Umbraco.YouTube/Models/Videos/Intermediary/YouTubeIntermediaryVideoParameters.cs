using System;
using Limbo.Umbraco.YouTube.Options;
using Newtonsoft.Json;
using Skybrud.Essentials.Json.Newtonsoft.Converters.Time;

namespace Limbo.Umbraco.YouTube.Models.Videos.Intermediary;

#pragma warning disable CS1591
public class YouTubeIntermediaryVideoParameters {

    /// <summary>
    /// Gets whether the video will automatically start to play when the player loads.
    ///
    /// If you enable Autoplay, playback will occur without any user interaction with the player; playback data
    /// collection and sharing will therefore occur upon page load.
    /// </summary>
    [JsonProperty("autoplay", NullValueHandling = NullValueHandling.Ignore)]
    public bool? Autoplay { get; }

    /// <summary>
    /// Gets whether the video player controls are displayed.
    /// </summary>
    [JsonProperty("controls", NullValueHandling = NullValueHandling.Ignore)]
    public bool? ShowControls { get; }

    /// <summary>
    /// Gets whether the player should not respond to keyboard controls.
    /// </summary>
    [JsonProperty("disablekb", NullValueHandling = NullValueHandling.Ignore)]
    public bool? DisableKeyboard { get; }

    /// <summary>
    /// Gets whether the player can be controlled via IFrame Player API calls.
    /// </summary>
    [JsonProperty("enablejsapi", NullValueHandling = NullValueHandling.Ignore)]
    public bool? EnableJsApi { get; }

    /// <summary>
    /// Gets whether the player should play the video again and again.
    /// </summary>
    [JsonProperty("loop", NullValueHandling = NullValueHandling.Ignore)]
    public bool? Loop { get; }

    /// <summary>
    /// Gets whether the player should show related videos when playback of the initial video ends. If
    /// <c>false</c>, related videos will come from the same channel as the video that was just played.
    /// </summary>
    [JsonProperty("rel", NullValueHandling = NullValueHandling.Ignore)]
    public bool? ShowRelated { get; }

    /// <summary>
    /// Gets the time offset at which the video should begin playing. The player looks for the closest
    /// keyframe at or before the time that you specify.
    /// </summary>
    [JsonProperty("start", NullValueHandling = NullValueHandling.Ignore)]
    [JsonConverter(typeof(TimeSpanConverter))]
    public TimeSpan? Start { get; }

    /// <summary>
    /// Gets the time, from the start of the video, when the player should stop playing the video.
    /// </summary>
    [JsonProperty("end", NullValueHandling = NullValueHandling.Ignore)]
    [JsonConverter(typeof(TimeSpanConverter))]
    public TimeSpan? End { get; }

    /// <summary>
    /// Gets or sets whether privacy-enhanced mode should be enabled. When you turn on privacy-enhanced mode,
    /// YouTube won't store information about visitors on your website unless they play the video.
    /// </summary>
    [JsonProperty("disableCookies", DefaultValueHandling = DefaultValueHandling.Ignore)]
    public bool? DisableCookies { get; }

    public YouTubeIntermediaryVideoParameters(YouTubeVideoOptions options) {
        Autoplay = options.Autoplay;
        ShowControls = options.ShowControls;
        DisableKeyboard = options.DisableKeyboard;
        EnableJsApi = options.EnableJsApi;
        Loop = options.Loop;
        ShowRelated = options.ShowRelated;
        Start = options.Start;
        End = options.End;
        DisableCookies = options.DisableCookies;
    }

    internal bool HasAny() {
        return Autoplay is not null || ShowControls is not null || DisableKeyboard is not null || EnableJsApi is not null || Loop is not null || ShowRelated is not null || Start is not null || End is not null || DisableCookies is not null;
    }

}