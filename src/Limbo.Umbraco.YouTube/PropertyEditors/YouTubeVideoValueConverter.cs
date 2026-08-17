// [CHANGE: Umbraco 17 upgrade - IPublishedPropertyType.DataType.Configuration is gone; configuration is read
// with ConfigurationAs<T>(), and the configured cache level now arrives as a string (see
// YouTubeVideoConfiguration).]
// [CHANGE: code review fix - the previous version of this comment claimed PropertyCacheLevel.Elements had been
// renamed to PropertyCacheLevel.Element. It wasn't: all five members still exist in Umbraco 17. What actually
// changed is the *fallback*, which went from Elements (v13) to Element - a deliberate behaviour change for data
// types that have no explicit "cacheLevel", documented in documentation/UPGRADE-UMBRACO-17.md.]
// Related: Api/YouTubeSecurityFilter.cs, wwwroot/Elements/Video.js, documentation/UPGRADE-UMBRACO-17.md

using System;
using Limbo.Umbraco.YouTube.Models;
using Newtonsoft.Json.Linq;
using Skybrud.Essentials.Json.Newtonsoft;
using Skybrud.Essentials.Json.Newtonsoft.Extensions;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Extensions;

#pragma warning disable 1591

namespace Limbo.Umbraco.YouTube.PropertyEditors;

/// <summary>
/// Property value converter for <see cref="YouTubeVideoPropertyEditor"/>.
/// </summary>
public class YouTubeVideoValueConverter : PropertyValueConverterBase {

    public override bool IsConverter(IPublishedPropertyType propertyType) {
        return propertyType.EditorAlias == YouTubeVideoPropertyEditor.EditorAlias;
    }

    public override object? ConvertSourceToIntermediate(IPublishedElement owner, IPublishedPropertyType propertyType, object? source, bool preview) {
        return source is string str && str.DetectIsJson() ? JsonUtils.ParseJsonObject(str) : null;
    }

    public override object? ConvertIntermediateToObject(IPublishedElement owner, IPublishedPropertyType propertyType, PropertyCacheLevel referenceCacheLevel, object? inter, bool preview) {
        if (inter is not JObject json || (json.GetObject("details") ?? json.GetObject("video")) is null) return null;
        return YouTubeVideoValue.Parse(json, propertyType.DataType.ConfigurationAs<YouTubeVideoConfiguration>());
    }

    public override Type GetPropertyValueType(IPublishedPropertyType propertyType) {
        return typeof(YouTubeVideoValue);
    }

    public override PropertyCacheLevel GetPropertyCacheLevel(IPublishedPropertyType propertyType) {

        // Default to "Element" if the configured value doesn't match a known cache level
        return Enum.TryParse(propertyType.DataType.ConfigurationAs<YouTubeVideoConfiguration>()?.CacheLevel, true, out PropertyCacheLevel cacheLevel)
            ? cacheLevel
            : PropertyCacheLevel.Element;

    }

}
