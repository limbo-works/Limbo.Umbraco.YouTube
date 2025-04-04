namespace Limbo.Umbraco.YouTube.Exceptions;

/// <summary>
/// Exception class thrown when a requested video is not found.
/// </summary>
public class YouTubeVideoNotFoundException : YouTubeIntermediaryVideoException {

    /// <summary>
    /// Initialize a new instance based on the specified <paramref name="source"/>.
    /// </summary>
    /// <param name="source">The source value - either a video URL or embed code.</param>
    public YouTubeVideoNotFoundException(string source) : base(source, "A video with the specified URL or embed code could not be found.") { }

}