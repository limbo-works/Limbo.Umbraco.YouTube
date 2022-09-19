using Limbo.Umbraco.Video.Models.Videos;
using Limbo.Umbraco.YouTube.Options;
using Microsoft.AspNetCore.Html;
using Newtonsoft.Json;
using Skybrud.Essentials.Json.Converters;

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
        /// Gets the HTML embed code.
        /// </summary>
        [JsonProperty("html")]
        [JsonConverter(typeof(StringJsonConverter))]
        public IHtmlContent Html { get; }

        /// <summary>
        /// Gets the player options.
        /// </summary>
        [JsonProperty("options", NullValueHandling = NullValueHandling.Ignore)]
        public YouTubeEmbedPlayerOptions? Options { get; }
        
        #endregion

        #region Constructors

        internal YouTubeEmbed(YouTubeVideoDetails video) {
            YouTubeEmbedOptions o = new(video, new YouTubeEmbedPlayerOptions());
            Url = o.GetEmbedUrl();
            Html = o.GetEmbedCode();
        }

        #endregion

    }

}