using Limbo.Umbraco.Video.Models.Videos;
using Skybrud.Social.Google.YouTube.Models.Videos;

namespace Limbo.Umbraco.YouTube.Models.Videos {

    /// <summary>
    /// Class representing a YouTube thumbnail.
    /// </summary>
    public class YouTubeThumbnail : VideoThumbnail {

        /// <summary>
        /// Initializes a new instance based on the specified <paramref name="thumbnail"/>.
        /// </summary>
        /// <param name="thumbnail">The video thumbnail from the YouTube API.</param>
        public YouTubeThumbnail(YouTubeVideoThumbnail thumbnail) : base(thumbnail.JObject) { }

    }

}