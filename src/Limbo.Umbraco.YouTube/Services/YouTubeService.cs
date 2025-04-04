using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.RegularExpressions;
using Limbo.Umbraco.YouTube.Exceptions;
using Limbo.Umbraco.YouTube.Models.Credentials;
using Limbo.Umbraco.YouTube.Models.Settings;
using Limbo.Umbraco.YouTube.Models.Videos.Intermediary;
using Limbo.Umbraco.YouTube.Options;
using Microsoft.Extensions.Options;
using Skybrud.Essentials.Collections.Extensions;
using Skybrud.Social.Google;
using Skybrud.Social.Google.YouTube;
using Skybrud.Social.Google.YouTube.Exceptions;
using Skybrud.Social.Google.YouTube.Models.Videos;
using Skybrud.Social.Google.YouTube.Options.Videos;
using Skybrud.Social.Google.YouTube.Responses.Videos;
using YouTubeException = Limbo.Umbraco.YouTube.Exceptions.YouTubeException;

namespace Limbo.Umbraco.YouTube.Services;

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
    public bool TryGetVideoId(string source, [NotNullWhen(true)] out YouTubeVideoOptions? options) {

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

        (_, string? query) = source.Split('?');

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

    /// <summary>
    /// Attempts to look up the video identified by the specified <paramref name="source"/>, and return an instance of <see cref="YouTubeIntermediaryVideoValue"/> if successful. When serialize to JSON, the value equals the property value saved in the database for properties using the YouTube video data type.
    /// </summary>
    /// <param name="source">The source (URL) as entered by the user.</param>
    /// <returns>An instance of <see cref="YouTubeIntermediaryVideoValue"/> if successful; otherwise, <see langword="null"/>.</returns>
    public virtual YouTubeIntermediaryVideoValue GetIntermediaryVideoValue(string source) {

        // Must have a source
        if (string.IsNullOrWhiteSpace(source)) throw new YouTubeException("No source specified.");

        // Try to parse the source
        if (!TryGetVideoId(source, out YouTubeVideoOptions? options)) throw new YouTubeInvalidSourceException(source);

        // Get the video from the options
        return GetIntermediaryVideoValue(source, options);

    }

    /// <summary>
    /// Attempts to look up the video identified by the specified <paramref name="source"/>, and return an instance of <see cref="YouTubeIntermediaryVideoValue"/> if successful. When serialize to JSON, the value equals the property value saved in the database for properties using the Skyfish video data type.
    /// </summary>
    /// <param name="source">The source (URL) as entered by the user.</param>
    /// <param name="options">The video options.</param>
    /// <returns>An instance of <see cref="YouTubeIntermediaryVideoValue"/> representing the video.</returns>
    protected virtual YouTubeIntermediaryVideoValue GetIntermediaryVideoValue(string source, YouTubeVideoOptions options) {

        // Get the first set of configured credentials (we don't currently support more than one)
        YouTubeCredentials? credentials = GetCredentials().FirstOrDefault();
        if (credentials == null || !TryGetHttpService(credentials, out YouTubeHttpService? http)) throw new YouTubeNotConfiguredException();

        // Initialize the options for the request to the YouTube API
        YouTubeGetVideoListOptions o = new(options.VideoId) {
            Part = YouTubeVideoParts.Snippet + YouTubeVideoParts.ContentDetails,
        };

        // Attempt to get video information from the YouTube API
        YouTubeVideo? video;
        try {
            YouTubeVideoListResponse response = http!.Videos.GetVideos(o);
            video = response.Body.Items.FirstOrDefault();
        } catch (YouTubeHttpException ex) {
            if (ex.Result.Error.Status == "PERMISSION_DENIED" && ex.Result.Error.Details.FirstOrDefault() is { Reason: "SERVICE_DISABLED" } d) {
                throw new YouTubeServiceDisabledException(d, ex);
            }
            throw new YouTubeIntermediaryVideoException(source, ex);
        } catch (Exception ex) {
            throw new YouTubeIntermediaryVideoException(source, ex);
        }

        // If the video isn't found, YouTube will return 200 OK and an empty list rather than 404 Not Found, so as
        // this won't be caught by the try/catch statement above, we can check whether "video" is null instead
        if (video == null) throw new YouTubeVideoNotFoundException(source);

        YouTubeIntermediaryVideoParameters? parameters = new(options);
        if (!parameters.HasAny()) parameters = null;

        // Initialize the intermediary details for the video
        YouTubeIntermediaryVideoDetails details = new(video);

        // Initialize a new intermediary video value
        return new YouTubeIntermediaryVideoValue(source, credentials, parameters, details);

    }

    #endregion

}