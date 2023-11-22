using DevExpress.Office.Utils;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.Commands;
using DevExpress.XtraRichEdit.Export;
using DevExpress.XtraRichEdit.Export.Html;
using TransfromService.RichText;

namespace TableEditor.RichTextEdit.CustomCommands
{
    public class CustomCopySelectionCommand : CopySelectionCommand
    {
        public CustomCopySelectionCommand(IRichEditControl control)
            : base(control)
        {
        }

        protected override void ExecuteCore()
        {
            var richEditControl = (RichEditControl)Control;
            richEditControl.BeforeExport += OnBeforeExport;
            string htmlForClipboard;

            //SetExportOptions(richEditControl.Options.Export.Html);

            try
            {
                var html = richEditControl.Document.GetHtmlContent(RichTextUtils.TextRange.Selection, richEditControl.Options.Export.Html);
                html = TransfromService.Utils.ProcessSpanTagStyleAttribute(html, true);

                htmlForClipboard = CF_HtmlHelper.GetHtmlClipboardFormat(html);
            }
            finally
            {
                richEditControl.BeforeExport -= OnBeforeExport;
            }

            var data = new DataObject();
            data.SetData(OfficeDataFormats.Rtf,
                richEditControl.Document.GetRtfText(richEditControl.Document.Selection));
            data.SetData(OfficeDataFormats.UnicodeText,
                richEditControl.Document.GetText(richEditControl.Document.Selection));
            data.SetData(OfficeDataFormats.Html, htmlForClipboard);

            Clipboard.Clear();
            Clipboard.SetDataObject(data, false);
        }

        void OnBeforeExport(object sender, BeforeExportEventArgs e)
        {
            if (e.Options is HtmlDocumentExporterOptions exporterOptions)
            {
                //SetExportOptions(exporterOptions);
            }
        }

        private void SetExportOptions(HtmlDocumentExporterOptions exportHtml)
        {
            exportHtml.ExportRootTag = ExportRootTag.Html;
            exportHtml.CssPropertiesExportType = CssPropertiesExportType.Inline;
            exportHtml.EmbedImages = false;
        }
    }
}