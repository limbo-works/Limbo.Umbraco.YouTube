using Skybrud.Social.Google.YouTube.Exceptions;
using Skybrud.Social.Google.YouTube.Models.Errors;

namespace Limbo.Umbraco.YouTube.Exceptions;

/// <summary>
/// Exception thrown when a response from the YouTube API indicates that access to the API is disabled for associated project.
/// </summary>
public class YouTubeServiceDisabledException : YouTubeException {

    /// <summary>
    /// Gets a reference to an object with more details about the error.
    /// </summary>
    public YouTubeErrorDetailsItem Details { get; }

    /// <summary>
    /// Initializes a new instance based on the specified <paramref name="details"/> and <paramref name="httpException"/>.
    /// </summary>
    /// <param name="details">An object with more details about the error.</param>
    /// <param name="httpException">An instance of <see cref="YouTubeHttpException"/> as received from the underlying API integration package.</param>
    public YouTubeServiceDisabledException(YouTubeErrorDetailsItem details, YouTubeHttpException httpException) : base("Access to the YouTube Data API v3 is currently disabled. Go to the Google Cloud Platform to enable access.", httpException) {
        Details = details;
    }

}