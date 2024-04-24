using DevExpress.XtraEditors;
using TransformService.TableMetadata;

namespace TextEditor.Editors.WorkbookEdit
{
    public partial class WorkbookUserControl : XtraUserControl, IEditorService, IClipboardService
    {
        public WorkbookUserControl()
        {
            InitializeComponent();
        }

        public TableMetadata GetTableMetadata()
        {
            throw new NotImplementedException();
        }

        public void SetTableMetadata(TableMetadata tableMetadata)
        {
            throw new NotImplementedException();
        }

        public ClipboardFormat GetClipboardFormat()
        {
            throw new NotImplementedException();
        }
    }
}