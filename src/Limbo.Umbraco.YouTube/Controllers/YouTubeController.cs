using System.Linq;
using Limbo.Umbraco.YouTube.Models.Credentials;
using Limbo.Umbraco.YouTube.Options;
using Limbo.Umbraco.YouTube.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Skybrud.Social.Google.YouTube.Options.Videos;
using Umbraco.Cms.Web.BackOffice.Controllers;
using Umbraco.Cms.Web.Common.Attributes;

#pragma warning disable 1591

namespace Limbo.Umbraco.YouTube.Controllers {

    [PluginController("Limbo")]
    public class YouTubeController : UmbracoAuthorizedApiController {

        private readonly YouTubeService _youTubeService;

        #region Constructors

        public YouTubeController(YouTubeService youTubeService) {
            _youTubeService = youTubeService;
        }

        #endregion

        #region Public API methods

        [HttpGet]
        [HttpPost]
        public object GetVideo() {

            // Get the "source" parameter from either GET or POST
            string source = HttpContext.Request.Query["source"];
            if (string.IsNullOrWhiteSpace(source) && HttpContext.Request.HasFormContentType) {
                source = HttpContext.Request.Form["source"].FirstOrDefault();
            }

            if (string.IsNullOrWhiteSpace(source)) return BadRequest("No source specified.");

            if (!_youTubeService.TryGetVideoId(source, out YouTubeVideoOptions options)) return BadRequest("Source doesn't match a valid URL or embed code.");

            YouTubeCredentials credentials = _youTubeService.GetCredentials().FirstOrDefault();
            if (credentials == null || !_youTubeService.TryGetHttpService(credentials, out var http)) return BadRequest("No credentials configured for YouTube.");

            var o = new YouTubeGetVideoListOptions(options.VideoId) {
                Part = YouTubeVideoParts.Snippet + YouTubeVideoParts.ContentDetails,
            };

            var response = http.Videos.GetVideos(o);

            var video = response.Body.Items.FirstOrDefault();

            if (video == null) return NotFound("Video not found.");

            JObject embed = JObject.FromObject(options);
            embed.Remove("videoId");

            JObject json = JObject.FromObject(new {
                credentials = new {
                    key = credentials.Key
                },
                video = video.JObject,
                embed
            });

            if (!embed.Properties().Any()) json.Remove("embed");
            
            return json;

        }

        #endregion

    }

}