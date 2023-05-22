using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Services;

#pragma warning disable 1591

namespace Limbo.Umbraco.YouTube.PropertyEditors {

    /// <summary>
    /// Represents a block list property editor.
    /// </summary>
    [DataEditor(EditorAlias, EditorName, EditorView, ValueType = ValueTypes.Json, Group = "Limbo", Icon = EditorIcon)]
    public class YouTubeEditor : DataEditor {

        private readonly IIOHelper _ioHelper;
        private readonly IEditorConfigurationParser _editorConfigurationParser;

        #region Constants

        internal const string EditorAlias = "Limbo.Umbraco.YouTube";

        internal const string EditorName = "Limbo YouTube Video";

        internal const string EditorView = "/App_Plugins/Limbo.Umbraco.YouTube/Views/Video.html";

        internal const string EditorIcon = "icon-limbo-youtube-alt color-limbo";

        #endregion

        #region Constructors

        public YouTubeEditor(IIOHelper ioHelper, IEditorConfigurationParser editorConfigurationParser, IDataValueEditorFactory dataValueEditorFactory) : base(dataValueEditorFactory) {
            _ioHelper = ioHelper;
            _editorConfigurationParser = editorConfigurationParser;
        }

        #endregion

        #region Member methods

        public override IDataValueEditor GetValueEditor(object? configuration) {
            IDataValueEditor editor = base.GetValueEditor(configuration);
            if (editor is DataValueEditor dve) dve.View += $"?v={YouTubePackage.InformationalVersion}";
            return editor;
        }

        protected override IConfigurationEditor CreateConfigurationEditor() {
            return new YouTubeConfigurationEditor(_ioHelper, _editorConfigurationParser);
        }

        #endregion

    }

}