using System;
using System.Diagnostics.CodeAnalysis;
using Limbo.Umbraco.YouTube.Models.Videos.Intermediary;

namespace Limbo.Umbraco.YouTube.Models.Api;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

public class VideoResult {

    public required string Source { get; init; }

    public required Guid Credentials { get; init; }

    public required VideoDetails Details { get; init; }

    public VideoResult() { }

    [SetsRequiredMembers]
    public VideoResult(YouTubeIntermediaryVideoValue value) {
        Source = value.Source;
        Credentials = value.Credentials;
        Details = new VideoDetails(value.Details);
    }

}