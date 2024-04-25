using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.Commands;
using DevExpress.XtraRichEdit.Services;

namespace TextEditor.Editors.RichTextEditor.CustomCommands
{
    internal class CustomRichEditCommandFactoryService : IRichEditCommandFactoryService
    {
        private readonly IRichEditCommandFactoryService _service;
        private readonly RichEditControl _control;
        private readonly IClipboardService _clipboardService;
        private readonly ITableMetadataManager _tableMetadataManager;

        public CustomRichEditCommandFactoryService(RichEditControl control, IClipboardService clipboardService,
            ITableMetadataManager tableMetadataManager, IRichEditCommandFactoryService service)
        {
            DevExpress.Utils.Guard.ArgumentNotNull(control, "control");
            DevExpress.Utils.Guard.ArgumentNotNull(clipboardService, "clipboardService");
            DevExpress.Utils.Guard.ArgumentNotNull(tableMetadataManager, "tableMetadataManager");
            DevExpress.Utils.Guard.ArgumentNotNull(service, "service");
            _control = control;
            _clipboardService = clipboardService;
            _tableMetadataManager = tableMetadataManager;
            _service = service;
        }

        public RichEditCommand CreateCommand(RichEditCommandId id)
        {
            if (id == RichEditCommandId.CopySelection /* && _useCustomCopy */)
                return new CustomCopySelectionCommand(_control, _clipboardService, _tableMetadataManager);

            /*
            if (id == RichEditCommandId.PasteSelection && _useCustomPaste)
                return new CustomPasteSelectionCommand(control);
            */

            return _service.CreateCommand(id);
        }
    }
}