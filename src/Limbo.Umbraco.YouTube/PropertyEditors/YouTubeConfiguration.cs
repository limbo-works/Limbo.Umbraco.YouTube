using Umbraco.Cms.Core.PropertyEditors;

#pragma warning disable CS1591

namespace Limbo.Umbraco.YouTube.PropertyEditors;

public class YouTubeConfiguration {

    /// <summary>
    /// Gets or sets the property cache level of the underlying property value converter. Defaults to <see cref="PropertyCacheLevel.Elements"/> if not specified.
    /// </summary>
    [ConfigurationField("cacheLevel", "Cache Level", "/App_Plugins/Limbo.Umbraco.Tables/Views/CacheLevel.html", Description = "Select the cache level of the underlying property value converter.")]
    public PropertyCacheLevel? CacheLevel { get; set; }

    /// <summary>
    /// Gets or sets whether embedded videos should automatically start playing.
    /// </summary>
    [ConfigurationField("autoplay",
        "Autoplay",
        $"/App_Plugins/{YouTubePackage.Alias}/Views/ButtonList.html?type={{alias}}",
        Description = "Select whether videos should autoplay when embedded.")]
    public bool? Autoplay { get; set; }

    /// <summary>
    /// Gets or sets whether embedded videos should loop.
    /// </summary>
    [ConfigurationField("loop",
        "Loop",
        $"/App_Plugins/{YouTubePackage.Alias}/Views/ButtonList.html?type={{alias}}",
        Description = "Select whether videos should loop.")]
    public bool? Loop { get; set; }

    [ConfigurationField("controls",
        "Show controls",
        $"/App_Plugins/{YouTubePackage.Alias}/Views/ButtonList.html?type={{alias}}",
        Description = "Select whether the video player controls are displayed.")]
    public bool? ShowControls { get; set; }

    [ConfigurationField("rel",
        "Show related",
        $"/App_Plugins/{YouTubePackage.Alias}/Views/ButtonList.html?type={{alias}}",
        Description = "Select the type of related videos to show when the video ends.")]
    public bool? ShowRelated { get; set; }

    [ConfigurationField("disableCookies",
        "Cookieless",
        $"/App_Plugins/{YouTubePackage.Alias}/Views/ButtonList.html?type={{alias}}",
        Description = "When you turn on privacy-enhanced mode, YouTube wont store information about visitors on your website unless they play the video.")]
    public bool? DisableCookies { get; set; }

    [ConfigurationField("hideLabel", "Hide label", "boolean", Description = "Select whether the label and description of properties using this data type should be hidden.<br /><br />Hiding the label and description can be useful in some cases - eg. to give the video picker a bit more horizontal space.")]
    public bool HideLabel { get; set; }

}