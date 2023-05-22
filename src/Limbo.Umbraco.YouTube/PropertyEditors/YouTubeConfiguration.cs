using Newtonsoft.Json;
using Umbraco.Cms.Core.PropertyEditors;

#pragma warning disable CS1591

namespace Limbo.Umbraco.YouTube.PropertyEditors {

    public class YouTubeConfiguration {

        [ConfigurationField("hideLabel", "Hide label", "boolean", Description = "Select whether the label and description of properties using this data type should be hidden.<br /><br />Hiding the label and description can be useful in some cases - eg. to give the video picker a bit more horizontal space.")]
        [JsonProperty("hideLabel")]
        public bool HideLabel { get; set; }

    }

}