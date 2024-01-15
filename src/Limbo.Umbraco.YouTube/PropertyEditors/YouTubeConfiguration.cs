using Newtonsoft.Json;
using Umbraco.Cms.Core.PropertyEditors;

#pragma warning disable CS1591

namespace Limbo.Umbraco.YouTube.PropertyEditors {

    public class YouTubeConfiguration {

        /// <summary>
        /// Gets or sets the property cache level of the underlying property value converter. Defaults to <see cref="PropertyCacheLevel.Elements"/> if not specified.
        /// </summary>
        [ConfigurationField("cacheLevel", "Cache Level", "/App_Plugins/Limbo.Umbraco.Tables/Views/CacheLevel.html", Description = "Select the cache level of the underlying property value converter.")]
        public PropertyCacheLevel? CacheLevel { get; set; }

        [ConfigurationField("hideLabel", "Hide label", "boolean", Description = "Select whether the label and description of properties using this data type should be hidden.<br /><br />Hiding the label and description can be useful in some cases - eg. to give the video picker a bit more horizontal space.")]
        public bool HideLabel { get; set; }

    }

}