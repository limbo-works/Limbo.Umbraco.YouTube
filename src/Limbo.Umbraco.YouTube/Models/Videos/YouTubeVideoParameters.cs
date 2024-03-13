using System;
using Limbo.Umbraco.YouTube.Json.Converters;
using Limbo.Umbraco.YouTube.PropertyEditors;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Skybrud.Essentials.Json.Newtonsoft.Extensions;

namespace Limbo.Umbraco.YouTube.Models.Videos {

    /// <summary>
    /// Class representing the parameters parsed from the source field of the <see cref="YouTubeEditor"/> property editor.
    /// </summary>
    public class YouTubeVideoParameters {

        #region Properties

        /// <summary>
        /// Gets whether embedded videos should automatically start playing.
        /// </summary>
        [JsonProperty("autoplay")]
        public bool? Autoplay { get; }

        /// <summary>
        /// Gets whether embedded videos should automatically start playing.
        /// </summary>
        [JsonProperty("loop")]
        public bool? Loop { get; }

        /// <summary>
        /// Gets or sets whether the video player controls are displayed.
        /// </summary>
        [JsonProperty("controls")]
        public bool? ShowControls { get; set; }

        /// <summary>
        /// Gets or sets whether the player should show related videos when playback of the initial video ends. If
        /// <c>false</c>, related videos will come from the same channel as the video that was just played.
        /// </summary>
        [JsonProperty("rel")]
        public bool? ShowRelated { get; set; }

        /// <summary>
        /// Gets or sets or sets whether privacy-enhached mode should be enabled. When you turn on privacy-enhanced mode,
        /// YouTube won't store information about visitors on your website unless they play the video.
        /// </summary>
        [JsonProperty("cookieless", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool? DisableCookies { get; set; }

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

        #endregion

        #region Constructors

        internal YouTubeVideoParameters(JObject? json) {
            Autoplay = json.GetBooleanOrNull("autoplay");
            Loop = json.GetBooleanOrNull("loop");
            ShowControls = json.GetBooleanOrNull("controls");
            ShowRelated = json.GetBooleanOrNull("rel");
            DisableCookies = json.GetBooleanOrNull("cookieless");
            Start = ParseSeconds(json.GetInt32OrNull("start"));
            End = ParseSeconds(json.GetInt32OrNull("end"));
        }

        #endregion

        #region Static methods

        internal static YouTubeVideoParameters Parse(JObject? json) {
            return new YouTubeVideoParameters(json);
        }

        private static TimeSpan? ParseSeconds(int? seconds) {
            return seconds is null ? null : TimeSpan.FromSeconds(seconds.Value);
        }

        #endregion

    }

}