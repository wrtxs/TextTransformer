using DevExpress.Spreadsheet;
using DevExpress.XtraRichEdit.API.Native;
using UnderlineType = DevExpress.Spreadsheet.UnderlineType;

namespace TextEditor.Editors.WorkbookEditor
{
    public partial class RichTextEditForm : DevExpress.XtraEditors.XtraForm
    {
        private Cell _cell;

        public RichTextEditForm()
        {
            InitializeComponent();

            KeyPreview = true;

            var image = DevExpress.Images.ImageResourceCache.Default.GetImage(
                Utils.GetNormalizedString("Office2013/Reports/ConvertToParagraphs_16x16.png"));

            if (image != null)
                Icon = Icon.FromHandle(((Bitmap)image).GetHicon());
        }

        public void SetCell(Cell cell)
        {
            _cell = cell;
            richEditControl1.Document.Delete(richEditControl1.Document.Range);

            richEditControl1.BeginUpdate();
            if (_cell.HasRichText)
            {
                var richText = _cell.GetRichText();
                var document = richEditControl1.Document;

                foreach (var run in richText.Runs)
                {
                    var range = document.InsertText(document.Range.End, run.Text);
                    var cp = document.BeginUpdateCharacters(range);
                    cp.Bold = run.Font.Bold;
                    cp.ForeColor = run.Font.Color;
                    cp.Italic = run.Font.Italic;
                    cp.FontName = run.Font.Name;
                    cp.FontSize = (float)run.Font.Size;
                    cp.Strikeout = run.Font.Strikethrough ? StrikeoutType.Single : StrikeoutType.None;

                    switch (run.Font.Script)
                    {
                        case ScriptType.Subscript:
                            cp.Subscript = true;
                            break;
                        case ScriptType.Superscript:
                            cp.Superscript = true;
                            break;
                        case ScriptType.None:
                        default:
                            cp.Subscript = false;
                            cp.Superscript = false;
                            break;
                    }

                    switch (run.Font.UnderlineType)
                    {
                        case UnderlineType.Single:
                            cp.Underline = DevExpress.XtraRichEdit.API.Native.UnderlineType.Single;
                            break;
                        case UnderlineType.Double:
                            cp.Underline = DevExpress.XtraRichEdit.API.Native.UnderlineType.Double;
                            break;
                        case UnderlineType.None:
                        case UnderlineType.SingleAccounting:
                        case UnderlineType.DoubleAccounting:
                        default:
                            cp.Underline = DevExpress.XtraRichEdit.API.Native.UnderlineType.None;
                            break;
                    }

                    document.EndUpdateCharacters(cp);
                }
            }
            else
            {
                richEditControl1.Text = _cell.DisplayText;
            }

            richEditControl1.Document.CaretPosition = richEditControl1.Document.Range.Start;
            richEditControl1.EndUpdate();
        }

        private void cmdSave_Click(object sender, EventArgs e)
        {
            var visitor = new CustomDocumentVisitor(richEditControl1.Document.Range.End.ToInt());
            var iterator = new DocumentIterator(richEditControl1.Document, true);

            while (iterator.MoveNext())
            {
                iterator.Current?.Accept(visitor);
            }

            var richText = visitor.RichText;
            _cell.SetRichText(richText);

            if (richEditControl1.Document.Paragraphs.Count > 1)
                _cell.Alignment.WrapText = true;
        }

        private void RichTextEditForm_Shown(object sender, EventArgs e)
        {
            cmdSave.Focus();
        }

        private void RichTextEditForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                Hide();
        }
    }
}