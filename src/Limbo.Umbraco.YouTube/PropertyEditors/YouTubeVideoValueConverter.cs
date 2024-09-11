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
/// Property value converter for <see cref="YouTubeVideoEditor"/>.
/// </summary>
public class YouTubeVideoValueConverter : PropertyValueConverterBase {

    public override bool IsConverter(IPublishedPropertyType propertyType) {
        return propertyType.EditorAlias == YouTubeVideoEditor.EditorAlias;
    }

    public override object? ConvertSourceToIntermediate(IPublishedElement owner, IPublishedPropertyType propertyType, object? source, bool preview) {
        return source is string str && str.DetectIsJson() ? JsonUtils.ParseJsonObject(str) : null;
    }

    public override object? ConvertIntermediateToObject(IPublishedElement owner, IPublishedPropertyType propertyType, PropertyCacheLevel referenceCacheLevel, object? inter, bool preview) {
        if (inter is not JObject json || json.GetObject("video") is null) return null;
        return YouTubeVideoValue.Parse(json, propertyType.DataType.Configuration as YouTubeVideoConfiguration);
    }

    public override Type GetPropertyValueType(IPublishedPropertyType propertyType) {
        return typeof(YouTubeVideoValue);
    }

    public override PropertyCacheLevel GetPropertyCacheLevel(IPublishedPropertyType propertyType) {

        // Default to "Elements" if configuration doesn't match (probably wouldn't happen)
        if (propertyType.DataType.Configuration is not YouTubeVideoConfiguration config) return PropertyCacheLevel.Elements;

        // Return the configured cachwe level (or "Elements" if not specified)
        return config.CacheLevel ?? PropertyCacheLevel.Elements;

    }

}