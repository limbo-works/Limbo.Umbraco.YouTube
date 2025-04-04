using System;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Limbo.Umbraco.YouTube.Exceptions;

public class YouTubeIntermediaryVideoException : YouTubeException {

    // TODO: find a better name for the class

    public new string Source { get; }

    public YouTubeIntermediaryVideoException(string source) : base("Failed retrieving video information from the YouTube API.") {
        Source = source;
    }

    public YouTubeIntermediaryVideoException(string source, Exception? innerException) : base("Failed retrieving video information from the YouTube API.", innerException) {
        Source = source;
    }

    public YouTubeIntermediaryVideoException(string source, string message) : base(message) {
        Source = source;
    }

    public YouTubeIntermediaryVideoException(string source, string message, Exception? innerException) : base(message, innerException) {
        Source = source;
    }

}