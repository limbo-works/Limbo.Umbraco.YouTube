using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using Limbo.Umbraco.YouTube.Models.Videos.Intermediary;
using Newtonsoft.Json;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Limbo.Umbraco.YouTube.Models.Api;

public class VideoDetails {

    [JsonProperty("_data")]
    [JsonPropertyName("_data")]
    public required string Data { get; init; }

    public VideoDetails() { }

    [SetsRequiredMembers]
    public VideoDetails(YouTubeIntermediaryVideoDetails details) {
        Data = details.Data.JObject.ToString(Formatting.None);
    }

}