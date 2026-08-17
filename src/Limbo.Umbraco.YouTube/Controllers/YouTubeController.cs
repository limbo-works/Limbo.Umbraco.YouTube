// [CHANGE: Umbraco 17 upgrade - UmbracoAuthorizedApiController/[PluginController] are gone. This is now a
// versioned Management API controller under /umbraco/management/api/v1/limbo/youtube. Responses are written
// with Newtonsoft (NewtonsoftJsonResult) because the intermediary models are annotated with [JsonProperty]
// and the Management API otherwise serializes with System.Text.Json.]
// Related: see documentation/UPGRADE-UMBRACO-17.md for the full list of changed files.

using System;
using Asp.Versioning;
using Limbo.Umbraco.YouTube.Api;
using Limbo.Umbraco.YouTube.Exceptions;
using Limbo.Umbraco.YouTube.Models;
using Limbo.Umbraco.YouTube.Models.Api;
using Limbo.Umbraco.YouTube.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Skybrud.Essentials.AspNetCore.Json.Newtonsoft;
using Skybrud.Essentials.Security.Extensions;
using Umbraco.Cms.Api.Common.Attributes;
using Umbraco.Cms.Api.Management.Controllers;
using Umbraco.Cms.Api.Management.Routing;
using Umbraco.Cms.Web.Common.Authorization;
using YouTubeException = Limbo.Umbraco.YouTube.Exceptions.YouTubeException;

#pragma warning disable 1591

namespace Limbo.Umbraco.YouTube.Controllers;

[ApiController]
[VersionedApiBackOfficeRoute(YouTubeApiConstants.Route)]
[Authorize(Policy = AuthorizationPolicies.SectionAccessContent)]
[MapToApi(YouTubeApiConstants.Alias)]
[ApiVersion("1.0")]
[ApiExplorerSettings(GroupName = YouTubeApiConstants.GroupName)]
public class YouTubeController : ManagementApiControllerBase {

    private readonly ILogger<YouTubeController> _logger;
    private readonly YouTubeService _youTubeService;

    #region Constructors

    public YouTubeController(ILogger<YouTubeController> logger, YouTubeService youTubeService) {
        _logger = logger;
        _youTubeService = youTubeService;
    }

    #endregion

    #region Public API methods

    /// <summary>
    /// Returns the server variables needed by the backoffice part of this package.
    /// </summary>
    /// <returns>An object with the server variables.</returns>
    [HttpGet("serverVariables")]
    public ServerVariables GetServerVariables() {
        return new ServerVariables {
            Version = YouTubePackage.InformationalVersion,
            CacheBuster = YouTubePackage.InformationalVersion.ToMd5Hash()
        };
    }

    /// <summary>
    /// Returns information about the video matching the specified <paramref name="source"/>.
    /// </summary>
    /// <param name="source">The source (URL or embed code) as entered by the user.</param>
    /// <returns>Information about the video matching <paramref name="source"/>.</returns>
    [HttpGet("video")]
    public object GetVideo(string? source) {

        if (string.IsNullOrWhiteSpace(source)) return BadRequest("No source specified.");

        try {
            return NewtonsoftJsonResult.Ok(_youTubeService.GetIntermediaryVideoValue(source));
        } catch (YouTubeInvalidSourceException ex) {
            return BadRequest(ex.Message);
        } catch (YouTubeVideoNotFoundException ex) {
            return NotFound(ex.Message);
        } catch (YouTubeException ex) {
            _logger.LogError(ex, "Failed retrieving video information from source {Source}", source);
            return InternalServerError(ex.Message);
        } catch (Exception ex) {
            _logger.LogError(ex, "Failed retrieving video information from source {Source}", source);
            return InternalServerError("Failed retrieving video information from the YouTube API.");
        }

    }

    #endregion

    #region Private methods

    private static IActionResult InternalServerError(object value) {
        return new ObjectResult(value) {
            StatusCode = StatusCodes.Status500InternalServerError
        };
    }

    #endregion

}
