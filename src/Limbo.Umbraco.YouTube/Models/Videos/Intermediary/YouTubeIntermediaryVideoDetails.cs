using Limbo.Umbraco.YouTube.Json.Newtonsoft.Converters;
using Newtonsoft.Json;
using Skybrud.Social.Google.YouTube.Models.Videos;

namespace Limbo.Umbraco.YouTube.Models.Videos.Intermediary;

#pragma warning disable CS1591

[JsonConverter(typeof(YouTubeJsonConverter))]
public class YouTubeIntermediaryVideoDetails {

    public YouTubeVideo Data { get; }

    public YouTubeIntermediaryVideoDetails(YouTubeVideo data) {
        Data = data;
    }

}