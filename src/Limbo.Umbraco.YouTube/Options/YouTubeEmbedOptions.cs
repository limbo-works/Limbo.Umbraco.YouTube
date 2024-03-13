using System.Web;
using Limbo.Umbraco.YouTube.Models.Videos;
using Microsoft.AspNetCore.Html;
using Newtonsoft.Json;
using Skybrud.Essentials.Common;

namespace Limbo.Umbraco.YouTube.Options;

/// <summary>
/// Class representing the embed options for a Vimeo video
/// </summary>
public class YouTubeEmbedOptions {

    #region Properties

    /// <summary>
    /// Gets or sets the ID of the video.
    /// </summary>
    [JsonProperty("id")]
    public string Id { get; set; }

    /// <summary>
    /// Gets or sets the title of the embed iframe.
    /// </summary>
    [JsonProperty("title", NullValueHandling = NullValueHandling.Ignore)]
    public string? Title { get; set; }

    /// <summary>
    /// Gets or sets the options for the player.
    /// </summary>
    [JsonProperty("player", NullValueHandling = NullValueHandling.Ignore)]
    public YouTubeEmbedPlayerOptions? Player { get; set; }

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance based on the specified <paramref name="id"/>.
    /// </summary>
    /// <param name="id">The ID of the video.</param>
    public YouTubeEmbedOptions(string id) {
        Id = id;
    }

    /// <summary>
    /// Initializes a new instance based on the specified <paramref name="video"/>.
    /// </summary>
    /// <param name="video">The Vimeo video.</param>
    public YouTubeEmbedOptions(YouTubeVideoDetails video) {
        Id = video.Id;
        Title = video.Snippet?.Title;
    }

    /// <summary>
    /// Initializes a new instance based on the specified <paramref name="video"/>.
    /// </summary>
    /// <param name="video">The Vimeo video.</param>
    /// <param name="player">The options for the YouTube video player.</param>
    public YouTubeEmbedOptions(YouTubeVideoDetails video, YouTubeEmbedPlayerOptions player) {
        Id = video.Id;
        Title = video.Snippet?.Title;
        Player = player;
    }

    #endregion

    #region Member methods

    /// <summary>
    /// Returns the embed URL for the video.
    /// </summary>
    /// <returns>The embed URL.</returns>
    public string GetEmbedUrl() {
        return (Player ?? new YouTubeEmbedPlayerOptions()).GetEmbedUrl(Id);
    }

    /// <summary>
    /// Returns the HTML embed code for the video described by this options instance.
    /// </summary>
    /// <returns>An instance of <see cref="HtmlString"/> representing the HTML embed code.</returns>
    public virtual HtmlString GetEmbedCode() {
        return GetEmbedCode(null, null);
    }

    /// <summary>
    /// Returns the HTML embed code for the video described by this options instance.
    /// </summary>
    /// <param name="width">The width of the video.</param>
    /// <param name="height">The height of the video.</param>
    /// <returns>An instance of <see cref="HtmlString"/> representing the HTML embed code.</returns>
    public virtual HtmlString GetEmbedCode(int? width, int? height) {

        width ??= 560;
        height ??= 315;

        if (string.IsNullOrWhiteSpace(Id)) throw new PropertyNotSetException(nameof(Id));

        string url = (Player ?? new YouTubeEmbedPlayerOptions()).GetEmbedUrl(Id);

        string title = string.IsNullOrWhiteSpace(Title) ? "Vimeo video player" : Title;

        string html = $"<iframe width=\"{width}\" height=\"{height}\" src=\"{HttpUtility.HtmlAttributeEncode(url)}\" title=\"{HttpUtility.HtmlAttributeEncode(title)}\" frameborder=\"0\" allow=\"accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture\" allowfullscreen></iframe>";

        return new HtmlString(html);

    }

    #endregion

}