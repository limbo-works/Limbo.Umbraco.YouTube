﻿using Limbo.Umbraco.YouTube.Services;
using Microsoft.AspNetCore.Html;
using Newtonsoft.Json;
using Skybrud.Essentials.Json.Converters;

namespace Limbo.Umbraco.YouTube.Models {
    
    /// <summary>
    /// Class representing the embed options of the video.
    /// </summary>
    public class YouTubeEmbed {

        #region Properties

        /// <summary>
        /// Gets the HTML embed code.
        /// </summary>
        [JsonProperty("html")]
        [JsonConverter(typeof(StringJsonConverter))]
        public HtmlString Html { get; }

        #endregion

        #region Constructors

        internal YouTubeEmbed(YouTubeVideoDetails video) {
            Html = new YouTubeVideoOptions(video.Id).GetEmbedCode();
        }

        #endregion

    }

}