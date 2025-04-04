namespace Limbo.Umbraco.YouTube.Exceptions;

/// <summary>
/// Exception class thrown when an invalid source is encountered.
/// </summary>
public class YouTubeInvalidSourceException : YouTubeIntermediaryVideoException {

    /// <summary>
    /// Gets the source value.
    /// </summary>
    public new string Source { get; }

    /// <summary>
    /// Initialize a new instance based on the specified <paramref name="source"/>.
    /// </summary>
    /// <param name="source">The source value - either a video URL or embed code.</param>
    public YouTubeInvalidSourceException(string source) : base("Source doesn't match a valid URL or embed code.") {
        Source = source;
    }

}