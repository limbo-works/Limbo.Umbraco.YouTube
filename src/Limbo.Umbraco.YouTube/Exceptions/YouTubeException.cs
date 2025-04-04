using System;

namespace Limbo.Umbraco.YouTube.Exceptions;

/// <summary>
/// Class representing a generic YouTube exception.
/// </summary>
public class YouTubeException : Exception {

    /// <summary>
    /// Initializes a new instance with a generic error message.
    /// </summary>
    public YouTubeException() : base("An error occured on the server.") { }

    /// <summary>
    /// Initializes a new instance with a generic error message and the specified <paramref name="innerException"/>.
    /// </summary>
    /// <param name="innerException">The inner exception.</param>
    public YouTubeException(Exception? innerException) : base("An error occured on the server.", innerException) { }

    /// <summary>
    /// Initializes a new instance based on the specified <paramref name="message"/>.
    /// </summary>
    /// <param name="message">The message of the exception.</param>
    public YouTubeException(string message) : base(message) { }

    /// <summary>
    /// Initializes a new instance based on the specified <paramref name="message"/> and <paramref name="innerException"/>.
    /// </summary>
    /// <param name="message">The message of the exception.</param>
    /// <param name="innerException">The inner exception.</param>
    public YouTubeException(string message, Exception? innerException) : base(message, innerException) { }

}