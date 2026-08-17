#pragma warning disable CS1591

using System;
using Limbo.Umbraco.YouTube.Models.Settings;
using Newtonsoft.Json;

namespace Limbo.Umbraco.YouTube.Models.Videos.Intermediary;

public class YouTubeIntermediaryVideoValue {

    [JsonProperty("source")]
    public string Source { get; }

    [JsonProperty("credentials")]
    public Guid Credentials { get; }

    [JsonProperty("parameters", NullValueHandling = NullValueHandling.Ignore)]
    public YouTubeIntermediaryVideoParameters? Parameters { get; }

    [JsonProperty("details")]
    public YouTubeIntermediaryVideoDetails Details { get; }

    public YouTubeIntermediaryVideoValue(string source, YouTubeCredentials credentials, YouTubeIntermediaryVideoParameters? parameters, YouTubeIntermediaryVideoDetails details) {
        Source = source;
        Parameters = parameters;
        Credentials = credentials.Key;
        Details = details;
    }

}