using System;
using Limbo.Umbraco.YouTube.Extensions;
using Limbo.Umbraco.YouTube.Json.Converters;
using Newtonsoft.Json;
using Skybrud.Essentials.Http.Collections;

namespace Limbo.Umbraco.YouTube.Options {

    /// <summary>
    /// Class with options describing a video.
    /// </summary>
    public class YouTubeVideoOptions {

        #region Properties

        /// <summary>
        /// Gets or sets the ID of the video.
        /// </summary>
        [JsonProperty("videoId")]
        public string VideoId { get; }

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
        /// Gets wether the player should not respond to keyboard controls.
        /// </summary>
        [JsonProperty("disablekb", NullValueHandling = NullValueHandling.Ignore)]
        public bool? DisableKeyboard { get; }

        /// <summary>
        /// Gets wether the player can be controlled via IFrame Player API calls.
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
        [JsonConverter(typeof(TimeSpanSecondsConverter))]
        public TimeSpan? Start { get; }

        /// <summary>
        /// Gets the time, from the start of the video, when the player should stop playing the video.
        /// </summary>
        [JsonProperty("end", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(TimeSpanSecondsConverter))]
        public TimeSpan? End { get; }

        /// <summary>
        /// Gets or sets whether privacy-enhached mode should be enabled. When you turn on privacy-enhanced mode,
        /// YouTube won't store information about visitors on your website unless they play the video.
        /// </summary>
        [JsonProperty("disableCookies", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool? DisableCookies { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance based on the specified <paramref name="videoId"/>.
        /// </summary>
        /// <param name="videoId">The ID of the video.</param>
        /// <param name="query">The query string.</param>
        /// <param name="disableCookies">Wheter cookies should be disabled (until the player is activated).</param>
        public YouTubeVideoOptions(string videoId, string query, bool? disableCookies) {
            
            VideoId = videoId;
            DisableCookies = disableCookies;

            IHttpQueryString q = HttpQueryString.Parse(query);

            if (q.TryGetBoolean("autoplay", out bool autoplay)) Autoplay = autoplay;
            if (q.TryGetBoolean("controls", out bool controls)) ShowControls = controls;
            if (q.TryGetBoolean("disablekb", out bool disablekb)) DisableKeyboard = disablekb;
            if (q.TryGetBoolean("enablejsapi", out bool enablejsapi)) EnableJsApi = enablejsapi;
            if (q.TryGetBoolean("loop", out bool loop)) Loop = loop;
            if (q.TryGetBoolean("rel", out bool rel)) ShowRelated = rel;
            if (q.TryGetDouble("start", out double start)) Start = TimeSpan.FromSeconds(start);
            if (q.TryGetDouble("end", out double end)) End = TimeSpan.FromSeconds(end);

        }

        #endregion

    }

}