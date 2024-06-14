using DevExpress.Spreadsheet;
using DevExpress.XtraRichEdit.API.Native;

namespace TextEditor.Editors.WorkbookEditor
{
    public class CustomDocumentVisitor : DocumentVisitorBase
    {
        private readonly RichTextString _richTextString;
        private readonly int _endPosition;
        public RichTextString RichText => _richTextString;

        public CustomDocumentVisitor(int endPos)
        {
            _richTextString = new RichTextString();
            _endPosition = endPos;
        }

        public override void Visit(DocumentText text)
        {
            base.Visit(text);
            RichTextRunFont runFont = CreateRichTextRun(text.TextProperties);
            _richTextString.AddTextRun(text.Text, runFont);
        }

        public override void Visit(DocumentParagraphEnd paragraphEnd)
        {
            base.Visit(paragraphEnd);
            if (_endPosition - 1 != paragraphEnd.Position)
            {
                RichTextRunFont runFont = CreateRichTextRun(paragraphEnd.TextProperties);
                _richTextString.AddTextRun(paragraphEnd.Text, runFont);
            }
        }

        private RichTextRunFont CreateRichTextRun(ReadOnlyTextProperties tp)
        {
            var runFont = new RichTextRunFont(tp.FontName, tp.DoubleFontSize / 2, tp.ForeColor)
            {
                Bold = tp.FontBold,
                Italic = tp.FontItalic,
                Strikethrough = tp.StrikeoutType == StrikeoutType.Single
            };

            switch (tp.Script)
            {
                case DevExpress.Office.CharacterFormattingScript.Subscript:
                    runFont.Script = ScriptType.Subscript;
                    break;
                case DevExpress.Office.CharacterFormattingScript.Superscript:
                    runFont.Script = ScriptType.Superscript;
                    break;
                default:
                    runFont.Script = ScriptType.None;
                    break;

            }

            switch (tp.UnderlineType)
            {
                case DevExpress.XtraRichEdit.API.Native.UnderlineType.Single:
                    runFont.UnderlineType = DevExpress.Spreadsheet.UnderlineType.Single;
                    break;
                case DevExpress.XtraRichEdit.API.Native.UnderlineType.Double:
                    runFont.UnderlineType = DevExpress.Spreadsheet.UnderlineType.Double;
                    break;
                default:
                    runFont.UnderlineType = DevExpress.Spreadsheet.UnderlineType.None;
                    break;
            }

            return runFont;
        }
    }
}