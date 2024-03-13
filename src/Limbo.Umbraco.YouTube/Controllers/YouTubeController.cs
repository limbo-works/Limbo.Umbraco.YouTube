using System;
using System.Linq;
using Limbo.Umbraco.YouTube.Models.Credentials;
using Limbo.Umbraco.YouTube.Options;
using Limbo.Umbraco.YouTube.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Skybrud.Social.Google.YouTube;
using Skybrud.Social.Google.YouTube.Models.Videos;
using Skybrud.Social.Google.YouTube.Options.Videos;
using Skybrud.Social.Google.YouTube.Responses.Videos;
using Umbraco.Cms.Web.BackOffice.Controllers;
using Umbraco.Cms.Web.Common.Attributes;

#pragma warning disable 1591

namespace Limbo.Umbraco.YouTube.Controllers;

[PluginController("Limbo")]
public class YouTubeController : UmbracoAuthorizedApiController {

    private readonly ILogger<YouTubeController> _logger;
    private readonly YouTubeService _youTubeService;

    #region Constructors

    public YouTubeController(ILogger<YouTubeController> logger, YouTubeService youTubeService) {
        _logger = logger;
        _youTubeService = youTubeService;
    }

    #endregion

    #region Public API methods

    [HttpGet]
    [HttpPost]
    public object GetVideo() {

        // Get the "source" parameter from either GET or POST
        string? source = HttpContext.Request.Query["source"];
        if (string.IsNullOrWhiteSpace(source) && HttpContext.Request.HasFormContentType) {
            source = HttpContext.Request.Form["source"].FirstOrDefault();
        }

        if (string.IsNullOrWhiteSpace(source)) return BadRequest("No source specified.");

        // Try to get thhe YouTube video ID from "source" - which be either an embed code or a URL
        if (!_youTubeService.TryGetVideoId(source, out YouTubeVideoOptions? options)) return BadRequest("Source doesn't match a valid URL or embed code.");

        // Get the first set of configured credentials (we don't currently support more than one)
        YouTubeCredentials? credentials = _youTubeService.GetCredentials().FirstOrDefault();
        if (credentials == null || !_youTubeService.TryGetHttpService(credentials, out YouTubeHttpService? http)) return BadRequest("No credentials configured for YouTube.");

        // Initialize the options for the request to the YouTube API
        YouTubeGetVideoListOptions o = new(options!.VideoId) {
            Part = YouTubeVideoParts.Snippet + YouTubeVideoParts.ContentDetails,
        };

        // Attempt to get video information from the YouTube API
        YouTubeVideo? video;
        try {
            YouTubeVideoListResponse response = http!.Videos.GetVideos(o);
            video = response.Body.Items.FirstOrDefault();
        } catch (Exception ex) {
            _logger.LogError(ex, "Failed retrieving video information for from source {Source}", source);
            return BadRequest("Failed retrieving video information from the YouTube API.");
        }

        // If the video isn't found, YouTube will return 200 OK and an empty list rather than 404 Not Found, so as
        // this won't be caught by the try/catch statement above, we can check whether "video" is null instead
        if (video == null) {
            return NotFound(source.Contains("<iframe") ? "A video with the specified embed code could not be found." : "A video with the specified URL could not be found.");
        }

        JObject embed = JObject.FromObject(options);
        embed.Remove("videoId");

        JObject parameters = new();
        if (options.Autoplay is not null) parameters.Add("autoplay", options.Autoplay.Value);
        if (options.Loop is not null) parameters.Add("loop", options.Loop.Value);
        if (options.DisableCookies is not null) parameters.Add("cookieless", options.DisableCookies.Value);
        if (options.ShowControls is not null) parameters.Add("controls", options.ShowControls.Value);
        if (options.ShowRelated is not null) parameters.Add("rel", options.ShowRelated.Value);
        if (options.Start is not null) parameters.Add("start", (int) options.Start.Value.TotalSeconds);
        if (options.End is not null) parameters.Add("end", (int) options.End.Value.TotalSeconds);

        JObject json = JObject.FromObject(new {
            credentials = new {
                key = credentials.Key
            },
            parameters,
            video = video.JObject,
            embed
        });

        if (!embed.Properties().Any()) json.Remove("embed");

        return json;

    }

    #endregion

}