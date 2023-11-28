using DevExpress.Office.Utils;
using DevExpress.XtraRichEdit.Commands;
using TransfromService.RichText;

namespace TableEditor.RichTextEdit.CustomCommands
{
    internal class CustomCopySelectionCommand : CopySelectionCommand
    {
        public CustomCopySelectionCommand(RichEditControlEx control)
            : base(control)
        {
        }

        protected override void ExecuteCore()
        {
            var richEditControl = (RichEditControlEx)Control;
            //richEditControl.BeforeExport += OnBeforeExport;
            string htmlForClipboard;

            //SetExportOptions(richEditControl.Options.Export.Html);

            var selRange = richEditControl.Document.GetSelectedRange();

            try
            {
                var html = richEditControl.Document.GetHtmlContent(selRange, richEditControl.GetTableTitle(), richEditControl.Options.Export.Html);
                html = TransfromService.Utils.ProcessSpanTagStyleAttribute(html, true);

                htmlForClipboard = CF_HtmlHelper.GetHtmlClipboardFormat(html);
            }
            finally
            {
                //richEditControl.BeforeExport -= OnBeforeExport;
            }

            var data = new DataObject();
            data.SetData(OfficeDataFormats.Rtf,
                richEditControl.Document.GetRtfText(selRange));
            data.SetData(OfficeDataFormats.UnicodeText,
                richEditControl.Document.GetText(selRange));
            data.SetData(OfficeDataFormats.Html, htmlForClipboard);

            Clipboard.Clear();
            Clipboard.SetDataObject(data, false);
        }

        //void OnBeforeExport(object sender, BeforeExportEventArgs e)
        //{
        //    if (e.Options is HtmlDocumentExporterOptions exporterOptions)
        //    {
        //        //SetExportOptions(exporterOptions);
        //    }
        //}

        //private void SetExportOptions(HtmlDocumentExporterOptions exportHtml)
        //{
        //    exportHtml.ExportRootTag = ExportRootTag.Html;
        //    exportHtml.CssPropertiesExportType = CssPropertiesExportType.Inline;
        //    exportHtml.EmbedImages = false;
        //}
    }
}