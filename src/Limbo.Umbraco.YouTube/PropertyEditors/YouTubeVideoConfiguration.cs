// [CHANGE: Umbraco 17 upgrade - [ConfigurationField] now only carries the alias; labels, descriptions and the
// editing UI for each field live in the propertyEditorSchema settings in wwwroot/EntryPoint.js. CacheLevel is
// stored as a string rather than a PropertyCacheLevel so it does not depend on how the configuration
// serializer handles enums; YouTubeVideoValueConverter maps it. The old "hideLabel" field is gone - the new
// backoffice controls label visibility on the document type property, not on the data type.]
// Related: see documentation/UPGRADE-UMBRACO-17.md for the full list of changed files.

using Umbraco.Cms.Core.PropertyEditors;

#pragma warning disable CS1591

namespace Limbo.Umbraco.YouTube.PropertyEditors;

public class YouTubeVideoConfiguration {

    /// <summary>
    /// Gets or sets the property cache level of the underlying property value converter. Defaults to
    /// <see cref="PropertyCacheLevel.Element"/> if not specified or not recognized.
    /// </summary>
    [ConfigurationField("cacheLevel")]
    public string? CacheLevel { get; set; }

    /// <summary>
    /// Gets or sets whether embedded videos should automatically start playing.
    /// </summary>
    [ConfigurationField("autoplay")]
    public bool? Autoplay { get; set; }

    /// <summary>
    /// Gets or sets whether embedded videos should loop.
    /// </summary>
    [ConfigurationField("loop")]
    public bool? Loop { get; set; }

    /// <summary>
    /// Gets or sets whether the video player controls should be displayed.
    /// </summary>
    [ConfigurationField("controls")]
    public bool? ShowControls { get; set; }

    /// <summary>
    /// Gets or sets whether the player should show related videos once the video has finished playing.
    /// </summary>
    [ConfigurationField("rel")]
    public bool? ShowRelated { get; set; }

    /// <summary>
    /// Gets or sets whether the embed code should use a cookieless player. When privacy-enhanced mode is turned
    /// on, YouTube won't store information about visitors on your website unless they play the video.
    /// </summary>
    [ConfigurationField("disableCookies")]
    public bool? DisableCookies { get; set; }

}
