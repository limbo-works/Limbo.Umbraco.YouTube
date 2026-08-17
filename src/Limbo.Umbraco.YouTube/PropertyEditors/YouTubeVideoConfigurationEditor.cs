// [CHANGE: Umbraco 17 upgrade - ConfigurationEditor<T> no longer takes an IEditorConfigurationParser, and
// configuration fields no longer carry a server-side "View". The editing UI for each field is declared in the
// propertyEditorSchema registration in wwwroot/EntryPoint.js instead, so the view-rewriting loop is gone.]
// Related: see documentation/UPGRADE-UMBRACO-17.md for the full list of changed files.

using System.Collections.Generic;
using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.PropertyEditors;

#pragma warning disable CS1591

namespace Limbo.Umbraco.YouTube.PropertyEditors;

public class YouTubeVideoConfigurationEditor : ConfigurationEditor<YouTubeVideoConfiguration> {

    public YouTubeVideoConfigurationEditor(IIOHelper ioHelper) : base(ioHelper) { }

    public override IDictionary<string, object> DefaultConfiguration => new Dictionary<string, object> {
        { "cacheLevel", nameof(PropertyCacheLevel.Element) }
    };

}
