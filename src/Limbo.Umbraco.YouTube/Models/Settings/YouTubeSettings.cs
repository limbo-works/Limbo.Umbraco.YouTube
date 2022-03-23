using Limbo.Umbraco.YouTube.Models.Credentials;
using Umbraco.Cms.Core.Configuration.Models;

namespace Limbo.Umbraco.YouTube.Models.Settings {

    /// <summary>
    /// Class representing the settings for this package.
    /// </summary>
    [UmbracoOptions("Limbo:YouTube", BindNonPublicProperties = true)]
    public class YouTubeSettings {

        /// <summary>
        /// Gets a collection of the credentials configured for YouTube.
        /// </summary>
        public YouTubeCredentials[] Credentials { get; internal set; }

    }

}