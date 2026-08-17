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