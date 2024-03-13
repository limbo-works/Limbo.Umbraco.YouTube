using System;
using System.Collections.Generic;
using System.Linq;
using Limbo.Umbraco.Video.Models.Videos;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Skybrud.Essentials.Json.Newtonsoft;
using Skybrud.Essentials.Json.Converters.Time;
using Skybrud.Essentials.Json.Newtonsoft.Extensions;
using Skybrud.Social.Google.YouTube.Models.Videos;
using Umbraco.Extensions;

namespace Limbo.Umbraco.YouTube.Models.Videos;

/// <summary>
/// Class with details about a YouTube video.
/// </summary>
public class YouTubeVideoDetails : IVideoDetails {

    #region Properties

    /// <summary>
    /// Gets a reference to the <see cref="YouTubeVideo"/> as received from the YouTube API.
    /// </summary>
    [JsonIgnore]
    public YouTubeVideo Data { get; }

    /// <summary>
    /// Gets the ID of the video.
    /// </summary>
    [JsonProperty("id")]
    public string Id => Data.Id;

    /// <summary>
    /// Gets the Vimeo URL of the video.
    /// </summary>
    [JsonProperty("url")]
    public string Url => $"https://www.youtube.com/watch?v={Id}";

    /// <summary>
    /// Gets the title of the video.
    /// </summary>
    [JsonProperty("title")]
    public string Title => Data.Snippet?.Title ?? string.Empty;

    /// <summary>
    /// Gets the description of the video.
    /// </summary>
    [JsonProperty("description")]
    public string? Description => Data.Snippet?.Description;

    /// <summary>
    /// Gets the duration of the video.
    /// </summary>
    [JsonProperty("duration")]
    [JsonConverter(typeof(TimeSpanSecondsConverter))]
    public TimeSpan Duration => Data.ContentDetails?.Duration.Value ?? TimeSpan.Zero;

    TimeSpan? IVideoDetails.Duration => Duration;

    /// <summary>
    /// Gets a list of thumbnails of the video.
    /// </summary>
    [JsonProperty("thumbnails", NullValueHandling = NullValueHandling.Ignore)]
    public IEnumerable<YouTubeThumbnail> Thumbnails { get; }

    /// <summary>
    /// Gets an array with the files of the video. This will currently always be empty.
    /// </summary>
    [JsonIgnore]
    public IEnumerable<IVideoFile> Files { get; }

    /// <summary>
    /// Gets a reference to the <strong>snippet</strong> part of the video.
    /// </summary>
    [JsonIgnore]
    public YouTubeVideoSnippet? Snippet => Data.Snippet;

    /// <summary>
    /// Gets whether the <see cref="Snippet"/> property was included in the response.
    /// </summary>
    [JsonIgnore]
    public bool HasSnippet => Snippet != null;

    /// <summary>
    /// Gets a reference to the <strong>contentDetails</strong> part of the video.
    /// </summary>
    [JsonIgnore]
    public YouTubeVideoContentDetails? ContentDetails => Data.ContentDetails;

    /// <summary>
    /// Gets whether the <see cref="ContentDetails"/> property was included in the response.
    /// </summary>
    [JsonIgnore]
    public bool HasContentDetails => ContentDetails != null;

    /// <summary>
    /// Gets a reference to the <strong>status</strong> part of the video.
    /// </summary>
    [JsonIgnore]
    public YouTubeVideoStatus? Status => Data.Status;

    /// <summary>
    /// Gets whether the <see cref="Status"/> property was included in the response.
    /// </summary>
    [JsonIgnore]
    public bool HasStatus => Status != null;

    /// <summary>
    /// Gets a reference to the <strong>statistics</strong> part of the video.
    /// </summary>
    [JsonIgnore]
    public YouTubeVideoStatistics? Statistics => Data.Statistics;

    /// <summary>
    /// Gets whether the <see cref="Statistics"/> property was included in the response.
    /// </summary>
    [JsonIgnore]
    public bool HasStatistics => Statistics != null;

    IEnumerable<IVideoThumbnail> IVideoDetails.Thumbnails => Thumbnails;

    #endregion

    #region Constructors

    private YouTubeVideoDetails(JObject json) {

        Data = json.GetString("_data", x => JsonUtils.ParseJsonObject(x, YouTubeVideo.Parse))!;

        YouTubeVideoThumbnails? thumbnails = Data.Snippet?.Thumbnails;

        if (thumbnails != null) {
            Thumbnails = new[] {
                thumbnails.Default, thumbnails.Medium, thumbnails.High, thumbnails.Standard, thumbnails.MaxRes
            }.WhereNotNull().Select(x => new YouTubeThumbnail(x));

        } else {
            Thumbnails = Array.Empty<YouTubeThumbnail>();
        }

        Files = Array.Empty<IVideoFile>();

    }

    #endregion

    #region Static methods

    /// <summary>
    /// Returns a new <see cref="YouTubeVideoDetails"/> parsed from the specified <paramref name="json"/>.
    /// </summary>
    /// <param name="json">The instance of <see cref="JObject"/> to parse.</param>
    /// <returns>An instance of <see cref="YouTubeVideoDetails"/>.</returns>
    public static YouTubeVideoDetails? Parse(JObject? json) {
        return json == null ? null : new YouTubeVideoDetails(json);
    }

    #endregion

}