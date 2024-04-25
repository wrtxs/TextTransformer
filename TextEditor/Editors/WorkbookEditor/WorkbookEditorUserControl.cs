using DevExpress.LookAndFeel;
using DevExpress.Spreadsheet;
using DevExpress.XtraEditors;
using DevExpress.XtraSpreadsheet.Menu;
using TransformService.TableMetadata;

namespace TextEditor.Editors.WorkbookEditor
{
    public sealed partial class WorkbookEditorUserControl : XtraUserControl, IEditorService, IClipboardService
    {
        private RichTextEditForm _richTextEditForm;    // Форма редактирования форматированного текста
        private SpreadsheetMenuItem _setRichTextItem;  // Команда контестного меню для вызова редактора форматированного текста

        public WorkbookEditorUserControl()
        {
            InitializeComponent();

            spreadsheetControl.LookAndFeel.UseDefaultLookAndFeel = false;
            spreadsheetControl.LookAndFeel.SetSkinStyle(SkinSvgPalette.DefaultSkin.PineLight);

            ribbonControl.SelectedPage = homeRibbonPage1;
            //ribbonControl.ForceInitialize();
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

        private void spreadsheetControl_CellBeginEdit(object sender,
            DevExpress.XtraSpreadsheet.SpreadsheetCellCancelEventArgs e)
        {
            if (e.Cell.HasRichText)
            {
                e.Cancel = true;
                ShowRichTextEditForm(e.Cell);
            }
        }

        private void ShowRichTextEditForm(Cell cell)
        {
            _richTextEditForm ??= new RichTextEditForm();
            _richTextEditForm.SetCell(cell);
            _richTextEditForm.ShowDialog();
        }

        private void spreadsheetControl_PopupMenuShowing(object sender,
            DevExpress.XtraSpreadsheet.PopupMenuShowingEventArgs e)
        {
            if (e.MenuType == DevExpress.XtraSpreadsheet.SpreadsheetMenuType.Cell)
            {
                Cell activeCell = spreadsheetControl.ActiveCell;
                if (activeCell.Value.IsEmpty || (!activeCell.HasFormula && activeCell.Value.IsText))
                {
                    if (_setRichTextItem == null)
                    {
                        _setRichTextItem = new SpreadsheetMenuItem("Форматированный текст...", SetRichTextItemClick);
                        _setRichTextItem.Image =
                            DevExpress.Images.ImageResourceCache.Default.GetImage(
                                Utils.GetNormalizedString("Office2013/Reports/ConvertToParagraphs_16x16.png"));
                    }

                    e.Menu.Items.Add(_setRichTextItem);
                }
            }
        }

        private void SetRichTextItemClick(object sender, EventArgs e)
        {
            ShowRichTextEditForm(spreadsheetControl.ActiveCell);
        }
    }
}