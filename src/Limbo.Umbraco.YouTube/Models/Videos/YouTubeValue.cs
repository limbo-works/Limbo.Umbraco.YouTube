using Limbo.Umbraco.Video.Models.Videos;
using Limbo.Umbraco.YouTube.PropertyEditors;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Skybrud.Essentials.Json.Extensions;

namespace Limbo.Umbraco.YouTube.Models.Videos {

    /// <summary>
    /// Class representing the value of the <see cref="YouTubeEditor"/> property editor.
    /// </summary>
    public class YouTubeValue : IVideoValue {

        #region Properties

        /// <summary>
        /// Gets the source (URL or embed code) as entered by the user.
        /// </summary>
        [JsonProperty("source")]
        public string Source { get; }

        /// <summary>
        /// Gets the details about the picked video.
        /// </summary>
        [JsonProperty("video")]
        public YouTubeVideoDetails Video { get; }

        /// <summary>
        /// Gets embed information for the video.
        /// </summary>
        [JsonProperty("embed")]
        public YouTubeEmbed Embed { get; }

        IVideoDetails IVideoValue.Video => Video;

        IVideoEmbed IVideoValue.Embed => Embed;

        #endregion

        #region Constructors

        private YouTubeValue(JObject json) {
            Source = json.GetString("source");
            Video = json.GetObject("video", YouTubeVideoDetails.Parse);
            Embed = new YouTubeEmbed(Video);
        }

        #endregion

        #region Static methods

        internal static YouTubeValue Parse(JObject json) {
            return json == null ? null : new YouTubeValue(json);
        }

        #endregion

    }

}