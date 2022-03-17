using Newtonsoft.Json.Linq;
using Skybrud.Essentials.Json.Extensions;
using Skybrud.Social.Google.YouTube.Models.Videos;

namespace Limbo.Umbraco.YouTube.Models {
    
    /// <summary>
    /// Class with details about a YouTube video.
    /// </summary>
    public class YouTubeVideoDetails {

        #region Properties

        /// <summary>
        /// Gets the ID of the video.
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// Gets a reference to the <strong>snippet</strong> part of the video.
        /// </summary>
        public YouTubeVideoSnippet Snippet { get; }

        /// <summary>
        /// Gets whether the <see cref="Snippet"/> property was included in the response.
        /// </summary>
        public bool HasSnippet => Snippet != null;
        
        /// <summary>
        /// Gets a reference to the <strong>contentDetails</strong> part of the video.
        /// </summary>
        public YouTubeVideoContentDetails ContentDetails { get; }

        /// <summary>
        /// Gets whether the <see cref="ContentDetails"/> property was included in the response.
        /// </summary>
        public bool HasContentDetails => ContentDetails != null;
        
        /// <summary>
        /// Gets a reference to the <strong>status</strong> part of the video.
        /// </summary>
        public YouTubeVideoStatus Status { get; }

        /// <summary>
        /// Gets whether the <see cref="Status"/> property was included in the response.
        /// </summary>
        public bool HasStatus => Status != null;
        
        /// <summary>
        /// Gets a reference to the <strong>statistics</strong> part of the video.
        /// </summary>
        public YouTubeVideoStatistics Statistics { get; }

        /// <summary>
        /// Gets whether the <see cref="Statistics"/> property was included in the response.
        /// </summary>
        public bool HasStatistics => Statistics != null;

        #endregion

        #region Constructors

        private YouTubeVideoDetails(JObject obj) {
            Id = obj.GetString("id");
            Snippet = obj.GetObject("snippet", YouTubeVideoSnippet.Parse);
            ContentDetails = obj.GetObject("contentDetails", YouTubeVideoContentDetails.Parse);
            Status = obj.GetObject("status", YouTubeVideoStatus.Parse);
            Statistics = obj.GetObject("statistics", YouTubeVideoStatistics.Parse);
        }

        #endregion

        #region Static methods

        /// <summary>
        /// Returns a new <see cref="YouTubeVideoDetails"/> parsed from the specified <paramref name="obj"/>.
        /// </summary>
        /// <param name="obj">The instance of <see cref="JObject"/> to parse.</param>
        /// <returns>An instance of <see cref="YouTubeVideoDetails"/>.</returns>
        public static YouTubeVideoDetails Parse(JObject obj) {
            return obj == null ? null : new YouTubeVideoDetails(obj);
        }

        #endregion
    
    }

}