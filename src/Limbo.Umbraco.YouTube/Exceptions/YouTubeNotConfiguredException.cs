namespace Limbo.Umbraco.YouTube.Exceptions;

/// <summary>
/// Exception class thrown when the YouTube package isn't configured.
/// </summary>
public class YouTubeNotConfiguredException : YouTubeException {

    /// <summary>
    /// Initialize a new instance.
    /// </summary>
    public YouTubeNotConfiguredException() : base("No YouTube credentials configured.") { }

}