// [CHANGE: Umbraco 17 upgrade - name, view, icon and group have moved out of [DataEditor] and into the
// "propertyEditorSchema" registration in wwwroot/EntryPoint.js. GetValueEditor() no longer needs to append a
// cache buster to a view URL, because there is no server-rendered view any more.]
// Related: see documentation/UPGRADE-UMBRACO-17.md for the full list of changed files.

using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.PropertyEditors;

#pragma warning disable 1591

namespace Limbo.Umbraco.YouTube.PropertyEditors;

/// <summary>
/// Represents the YouTube video property editor.
/// </summary>
[DataEditor(EditorAlias, ValueType = ValueTypes.Json)]
public class YouTubeVideoPropertyEditor : DataEditor {

    private readonly IIOHelper _ioHelper;

    #region Constants

    public const string EditorAlias = "Limbo.Umbraco.YouTube";

    public const string EditorName = "Limbo YouTube Video";

    public const string EditorIcon = "limbo-youtube-alt";

    #endregion

    #region Constructors

    public YouTubeVideoPropertyEditor(IDataValueEditorFactory dataValueEditorFactory, IIOHelper ioHelper) : base(dataValueEditorFactory) {
        _ioHelper = ioHelper;
        SupportsReadOnly = true;
    }

    #endregion

    #region Member methods

    protected override IConfigurationEditor CreateConfigurationEditor() {
        return new YouTubeVideoConfigurationEditor(_ioHelper);
    }

    #endregion

}
