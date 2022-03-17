using Umbraco.Cms.Core.PropertyEditors;

#pragma warning disable 1591

namespace Limbo.Umbraco.YouTube.PropertyEditors {

    /// <summary>
    /// Represents a block list property editor.
    /// </summary>
    [DataEditor(EditorAlias, EditorName, EditorView, ValueType = ValueTypes.Json, Group = "Limbo", Icon = EditorIcon)]
    public class YouTubeEditor : DataEditor {

        #region Constants

        internal const string EditorAlias = "Limbo.Umbraco.YouTube";

        internal const string EditorName = "Limbo YouTube Video";

        internal const string EditorView = "/App_Plugins/Limbo.Umbraco.YouTube/Views/Video.html";

        internal const string EditorIcon = "icon-youtube color-limbo";

        #endregion

        #region Constructors

        public YouTubeEditor(IDataValueEditorFactory dataValueEditorFactory) : base(dataValueEditorFactory) { }

        #endregion

    }

}