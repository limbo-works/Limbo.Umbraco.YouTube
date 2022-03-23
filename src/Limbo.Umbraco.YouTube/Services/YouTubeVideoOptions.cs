using System;
using Microsoft.AspNetCore.Html;
using Newtonsoft.Json;
using Skybrud.Essentials.Common;
using Skybrud.Essentials.Http.Collections;

namespace Limbo.Umbraco.YouTube.Services {

    /// <summary>
    /// Class with options describing a video.
    /// </summary>
    public class YouTubeVideoOptions {

        #region Properties

        /// <summary>
        /// Gets or sets the ID of the video.
        /// </summary>
        [JsonProperty("videoId")]
        public string VideoId { get; set; }

        /// <summary>
        /// Gets or sets the time offset at which the video should begin playing. The player looks for the closest
        /// keyframe at or before the time that you specify.
        /// </summary>
        [JsonProperty("start", NullValueHandling = NullValueHandling.Ignore)]
        public TimeSpan? Start { get; set; }

        /// <summary>
        /// Gets or sets whether privacy-enhached mode should be enabled. When you turn on privacy-enhanced mode,
        /// YouTube won't store information about visitors on your website unless they play the video.
        /// </summary>
        [JsonProperty("disableCookies", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool DisableCookies { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance based on the specified <paramref name="videoId"/>.
        /// </summary>
        /// <param name="videoId">The ID of the video.</param>
        public YouTubeVideoOptions(string videoId) {
            VideoId = videoId;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Returns the HTML embed code for the video described by this options instance.
        /// </summary>
        /// <returns>An instance of <see cref="HtmlString"/> representing the HTML embed code.</returns>
        public virtual HtmlString GetEmbedCode() {
            return GetEmbedCode(null, null);
        }

        /// <summary>
        /// Returns the HTML embed code for the video described by this options instance.
        /// </summary>
        /// <param name="width">The width of the video.</param>
        /// <param name="height">The height of the video.</param>
        /// <returns>An instance of <see cref="HtmlString"/> representing the HTML embed code.</returns>
        public virtual HtmlString GetEmbedCode(int? width, int? height) {

            width ??= 560;
            height ??= 315;

            if (string.IsNullOrWhiteSpace(VideoId)) throw new PropertyNotSetException(nameof(VideoId));

            IHttpQueryString query = new HttpQueryString();
            if (Start != null) query.Add("start", Start.Value.TotalSeconds);

            string url = $"https://www.{(DisableCookies ? "youtube-nocookie" : "youtube")}.com/embed/{VideoId}?{query}".TrimEnd('?');

            string html = $"<iframe width=\"{width}\" height=\"{height}\" src=\"{url}\" title=\"YouTube video player\" frameborder=\"0\" allow=\"accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture\" allowfullscreen></iframe>";

            return new HtmlString(html);

        }

        #endregion

    }

}