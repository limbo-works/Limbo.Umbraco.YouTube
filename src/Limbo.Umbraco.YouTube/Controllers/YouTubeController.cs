using System;
using System.Linq;
using Limbo.Umbraco.YouTube.Exceptions;
using Limbo.Umbraco.YouTube.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Umbraco.Cms.Web.BackOffice.Controllers;
using Umbraco.Cms.Web.Common.Attributes;
using YouTubeException = Limbo.Umbraco.YouTube.Exceptions.YouTubeException;

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

        try {
            return _youTubeService.GetIntermediaryVideoValue(source);
        } catch (YouTubeInvalidSourceException ex) {
            return BadRequest(ex.Message);
        } catch (YouTubeVideoNotFoundException ex) {
            return NotFound(ex.Message);
        } catch (YouTubeException ex) {
            _logger.LogError(ex, "Failed retrieving video information for from source {Source}", source);
            return BadRequest(ex.Message);
        } catch (Exception ex) {
            _logger.LogError(ex, "Failed retrieving video information for from source {Source}", source);
            return BadRequest("Failed retrieving video information from the YouTube API.");
        }

    }

    #endregion

}