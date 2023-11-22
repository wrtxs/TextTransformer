using DevExpress.XtraRichEdit.Commands;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.Services;

namespace TableEditor.RichTextEdit.CustomCommands
{
    internal class CustomRichEditCommandFactoryService : IRichEditCommandFactoryService
    {
        private readonly IRichEditCommandFactoryService _service;
        private readonly RichEditControl _control;

        public CustomRichEditCommandFactoryService(RichEditControl control, IRichEditCommandFactoryService service)
        {
            DevExpress.Utils.Guard.ArgumentNotNull(control, "control");
            DevExpress.Utils.Guard.ArgumentNotNull(service, "service");
            _control = control;
            _service = service;
        }

        public RichEditCommand CreateCommand(RichEditCommandId id)
        {
            if (id == RichEditCommandId.CopySelection /* && _useCustomCopy */)
                return new CustomCopySelectionCommand(_control);

            /*
            if (id == RichEditCommandId.PasteSelection && _useCustomPaste)
                return new CustomPasteSelectionCommand(control);
            */

            return _service.CreateCommand(id);
        }
    }
}
