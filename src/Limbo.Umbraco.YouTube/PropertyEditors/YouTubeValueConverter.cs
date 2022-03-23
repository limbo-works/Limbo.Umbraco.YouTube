using System;
using Limbo.Umbraco.YouTube.Models.Videos;
using Newtonsoft.Json.Linq;
using Skybrud.Essentials.Json;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Extensions;

#pragma warning disable 1591

namespace Limbo.Umbraco.YouTube.PropertyEditors {

    /// <summary>
    /// Property value converter for <see cref="YouTubeEditor"/>.
    /// </summary>
    public class YouTubeValueConverter : PropertyValueConverterBase {

        public override bool IsConverter(IPublishedPropertyType propertyType) {
            return propertyType.EditorAlias == YouTubeEditor.EditorAlias;
        }

        public override object ConvertSourceToIntermediate(IPublishedElement owner, IPublishedPropertyType propertyType, object source, bool preview) {
            return source is string str && str.DetectIsJson() ? JsonUtils.ParseJsonObject(str) : null;
        }

        public override object ConvertIntermediateToObject(IPublishedElement owner, IPublishedPropertyType propertyType, PropertyCacheLevel referenceCacheLevel, object inter, bool preview) {
            return YouTubeValue.Parse(inter as JObject);
        }

        public override object ConvertIntermediateToXPath(IPublishedElement owner, IPublishedPropertyType propertyType, PropertyCacheLevel referenceCacheLevel, object inter, bool preview) {
            return null;
        }

        public override Type GetPropertyValueType(IPublishedPropertyType propertyType) {
            return typeof(YouTubeValue);
        }

        public override PropertyCacheLevel GetPropertyCacheLevel(IPublishedPropertyType propertyType) {
            return PropertyCacheLevel.Element;
        }

    }

}