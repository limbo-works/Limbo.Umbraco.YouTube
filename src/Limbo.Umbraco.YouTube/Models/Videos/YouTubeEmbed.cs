using System.Globalization;
using Limbo.Umbraco.Video.Models.Videos;
using Limbo.Umbraco.YouTube.Options;
using Limbo.Umbraco.YouTube.PropertyEditors;
using Microsoft.AspNetCore.Html;
using Newtonsoft.Json;
using Skybrud.Essentials.Json.Newtonsoft.Converters;

namespace Limbo.Umbraco.YouTube.Models.Videos {

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

        [JsonProperty("controls", NullValueHandling = NullValueHandling.Ignore)]
        public bool? ShowControls { get; }

        [JsonProperty("rel", NullValueHandling = NullValueHandling.Ignore)]
        public bool? ShowRelated { get; }

        [JsonProperty("cookieless", NullValueHandling = NullValueHandling.Ignore)]
        public bool? DisableCookies { get; }

        [JsonProperty("start", NullValueHandling = NullValueHandling.Ignore)]
        public string? Start { get; }

        [JsonProperty("end", NullValueHandling = NullValueHandling.Ignore)]
        public string? End { get; }

        /// <summary>
        /// Gets the HTML embed code.
        /// </summary>
        [JsonProperty("html")]
        [JsonConverter(typeof(StringJsonConverter))]
        public IHtmlContent Html { get; }


        public YouTubeConfiguration? Config { get; }

        #endregion

        #region Constructors

        internal YouTubeEmbed(YouTubeVideoDetails video, YouTubeVideoParameters parameters, YouTubeConfiguration? config) {

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

}