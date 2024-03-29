﻿using System;
using Limbo.Umbraco.YouTube.Models.Videos;
using Newtonsoft.Json.Linq;
using Skybrud.Essentials.Json.Newtonsoft;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Extensions;

#pragma warning disable 1591

namespace Limbo.Umbraco.YouTube.PropertyEditors;

/// <summary>
/// Property value converter for <see cref="YouTubeEditor"/>.
/// </summary>
public class YouTubeValueConverter : PropertyValueConverterBase {

    public override bool IsConverter(IPublishedPropertyType propertyType) {
        return propertyType.EditorAlias == YouTubeEditor.EditorAlias;
    }

    public override object? ConvertSourceToIntermediate(IPublishedElement owner, IPublishedPropertyType propertyType, object? source, bool preview) {
        return source is string str && str.DetectIsJson() ? JsonUtils.ParseJsonObject(str) : null;
    }

    public override object? ConvertIntermediateToObject(IPublishedElement owner, IPublishedPropertyType propertyType, PropertyCacheLevel referenceCacheLevel, object? inter, bool preview) {
        return YouTubeValue.Parse(inter as JObject, propertyType.DataType.Configuration as YouTubeConfiguration);
    }

    public override Type GetPropertyValueType(IPublishedPropertyType propertyType) {
        return typeof(YouTubeValue);
    }

    public override PropertyCacheLevel GetPropertyCacheLevel(IPublishedPropertyType propertyType) {

        // Default to "Elements" if configuration doesn't match (probably wouldn't happen)
        if (propertyType.DataType.Configuration is not YouTubeConfiguration config) return PropertyCacheLevel.Elements;

        // Return the configured cachwe level (or "Elements" if not specified)
        return config.CacheLevel ?? PropertyCacheLevel.Elements;

    }

}