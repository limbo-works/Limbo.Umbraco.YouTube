using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Limbo.Umbraco.YouTube.Models.Credentials;
using Limbo.Umbraco.YouTube.Models.Settings;
using Limbo.Umbraco.YouTube.Options;
using Microsoft.Extensions.Options;
using Skybrud.Essentials.Collections.Extensions;
using Skybrud.Social.Google;
using Skybrud.Social.Google.YouTube;

namespace Limbo.Umbraco.YouTube.Services {

    /// <summary>
    /// Service for working with the YouTube integration.
    /// </summary>
    public class YouTubeService {

        private readonly IOptions<YouTubeSettings> _settings;

        #region Constructors

        /// <summary>
        /// Initializes a new instance based on the specified dependencies.
        /// </summary>
        public YouTubeService(IOptions<YouTubeSettings> settings) {
            _settings = settings;
        }

        #endregion

        #region Member methods

        /// <summary>
        /// Returns whether the specified <paramref name="source"/> is recognized as a YouTube URL or embed code.
        /// </summary>
        /// <param name="source">The source </param>
        /// <param name="options"></param>
        /// <returns></returns>
        public bool TryGetVideoId(string source, out YouTubeVideoOptions? options) {

            options = null;
            if (string.IsNullOrWhiteSpace(source)) return false;

            // Embed options
            bool? cookieless = source.Contains("youtube-nocookie.com") ? true : null;

            // Is "source" an iframe?
            if (source.StartsWith("<iframe")) {

                // Match the "src" attribute
                Match m0 = Regex.Match(source, "src=\"(.+?)\"", RegexOptions.IgnoreCase);
                if (m0.Success == false) return false;

                // Update the source with the value from the "src" attribute
                source = m0.Groups[1].Value;

            }

            // Does "source" match known formats of YouTube video URLs?
            Match m1 = Regex.Match(source, @"youtu(?:\.be|be\.com|be-nocookie\.com)/(embed/|)(?:.*v(?:/|=)|(?:.*/)?)([a-zA-Z0-9-_]+)", RegexOptions.IgnoreCase);
            if (m1.Success == false) return false;

            // Get the video ID from the regex
            string videoId = m1.Groups[2].Value;

            (_, string query) = source.Split('?');

            options = new YouTubeVideoOptions(videoId, query, cookieless);

            return true;

        }

        /// <summary>
        /// Returns a list of YouTube credentials.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<YouTubeCredentials> GetCredentials() {
            return _settings.Value.Credentials;
        }

        /// <summary>
        /// Creates a new HTTP service for accessing the YouTube API using the specified <paramref name="credentials"/>.
        /// </summary>
        /// <param name="credentials">The credentials.</param>
        /// <param name="http">When this method returns, holds the created HTTP service if successful; otherwise, <c>null</c>.</param>
        /// <returns><c>true</c> if successful; otherwise, <c>false</c>.</returns>
        public virtual bool TryGetHttpService(YouTubeCredentials credentials, out YouTubeHttpService? http) {

            if (credentials == null) throw new ArgumentNullException(nameof(credentials));

            //if (!string.IsNullOrWhiteSpace(credentials.ClientId) && !string.IsNullOrWhiteSpace(credentials.ClientSecret) && !string.IsNullOrWhiteSpace(credentials.RefreshToken)) {
            //    http = GoogleService.CreateFromRefreshToken(credentials.ClientId, credentials.ClientSecret, credentials.RefreshToken).YouTube();
            //    return true;
            //}

            if (!string.IsNullOrWhiteSpace(credentials.ApiKey)) {
                http = GoogleHttpService.CreateFromApiKey(credentials.ApiKey).YouTube();
                return true;
            }

            http = null;
            return false;

        }

        #endregion

    }

}