using System;
using Limbo.Umbraco.YouTube.Json.Converters;
using Newtonsoft.Json;
using Skybrud.Essentials.Http.Collections;

namespace Limbo.Umbraco.YouTube.Options {
    
    /// <summary>
    /// Class representing the options for the YouTube video player.
    /// </summary>
    public class YouTubeEmbedPlayerOptions {

        #region Properties

        /// <summary>
        /// Gets or sets whether the video will automatically start to play when the player loads.
        ///
        /// If you enable Autoplay, playback will occur without any user interaction with the player; playback data
        /// collection and sharing will therefore occur upon page load.
        /// </summary>
        [JsonProperty("autoplay")]
        public bool? Autoplay { get; set; }

        /// <summary>
        /// Gets or sets whether the video player controls are displayed.
        /// </summary>
        [JsonProperty("controls")]
        public bool? ShowControls { get; set; }

        /// <summary>
        /// Gets or sets wether the player should not respond to keyboard controls.
        /// </summary>
        [JsonProperty("disablekb")]
        public bool? DisableKeyboard { get; set; }

        /// <summary>
        /// Gets or sets wether the player can be controlled via IFrame Player API calls.
        /// </summary>
        [JsonProperty("enablejsapi")]
        public bool? EnableJsApi { get; set; }

        /// <summary>
        /// Gets or sets whether the player should play the video again and again.
        /// </summary>
        [JsonProperty("loop")]
        public bool? Loop { get; set; }

        /// <summary>
        /// Gets or sets whether the player should show related videos when playback of the initial video ends. If
        /// <c>false</c>, related videos will come from the same channel as the video that was just played.
        /// </summary>
        [JsonProperty("rel")]
        public bool? ShowRelated { get; set; }

        /// <summary>
        /// Gets or sets the time offset at which the video should begin playing. The player looks for the closest
        /// keyframe at or before the time that you specify.
        /// </summary>
        [JsonProperty("start", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(TimeSpanSecondsConverter))]
        public TimeSpan? Start { get; set; }

        /// <summary>
        /// Gets or sets the time, from the start of the video, when the player should stop playing the video.
        /// </summary>
        [JsonProperty("end", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(TimeSpanSecondsConverter))]
        public TimeSpan? End { get; set; }

        /// <summary>
        /// Gets or sets or sets whether privacy-enhached mode should be enabled. When you turn on privacy-enhanced mode,
        /// YouTube won't store information about visitors on your website unless they play the video.
        /// </summary>
        [JsonProperty("disableCookies", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool? DisableCookies { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance with default options.
        /// </summary>
        public YouTubeEmbedPlayerOptions() { }

        /// <summary>
        /// Initializes a new instance based on the specified <paramref name="options"/>.
        /// </summary>
        /// <param name="options">The options.</param>
        public YouTubeEmbedPlayerOptions(YouTubeVideoOptions options) {
            Start = options.Start;
            DisableCookies = options.DisableCookies;
        }

        #endregion

        #region Member methods

        /// <summary>
        /// Returns the embed URL for the video with the specified <paramref name="videoId"/>.
        /// </summary>
        /// <param name="videoId">The ID of the video.</param>
        /// <returns>The embed URL.</returns>
        public string GetEmbedUrl(string videoId) {

            if (string.IsNullOrWhiteSpace(videoId)) throw new ArgumentNullException(nameof(videoId));

            IHttpQueryString query = new HttpQueryString();

            AppendToQueryString(query);

            bool cookieless = DisableCookies != null && DisableCookies.Value;

            return $"https://www.{(cookieless ? "youtube-nocookie" : "youtube")}.com/embed/{videoId}?{query}".TrimEnd('?');

        }

        /// <summary>
        /// Appends the player options to the specified <paramref name="query"/> string.
        /// </summary>
        /// <param name="query">The query string.</param>
        public void AppendToQueryString(IHttpQueryString query) {
            if (Autoplay != null) query.Add("autoplay", Autoplay.Value ? "1" : "0");
            if (ShowControls != null) query.Add("controls", ShowControls.Value ? "1" : "0");
            if (DisableKeyboard != null) query.Add("disablekb", DisableKeyboard.Value ? "1" : "0");
            if (EnableJsApi != null) query.Add("enablejsapi", EnableJsApi.Value ? "1" : "0");
            if (Loop != null) query.Add("loop", Loop.Value ? "1" : "0");
            if (ShowRelated != null) query.Add("rel", ShowRelated.Value ? "1" : "0");
            if (Start != null) query.Add("start", Start.Value.TotalSeconds);
            if (End != null) query.Add("end", End.Value.TotalSeconds);
        }

        #endregion

    }

}