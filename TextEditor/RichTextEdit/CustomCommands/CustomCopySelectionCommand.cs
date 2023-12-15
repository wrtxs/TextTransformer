using DevExpress.Office.Utils;
using DevExpress.XtraRichEdit.Commands;
using TransfromService.RichText;

namespace TextEditor.RichTextEdit.CustomCommands
{
    internal class CustomCopySelectionCommand : CopySelectionCommand
    {
        public CustomCopySelectionCommand(RichEditControlEx control)
            : base(control)
        {
        }

        protected override void ExecuteCore()
        {
            if (Control is not RichEditControlEx richEditControl)
                return;

            //richEditControl.BeforeExport += OnBeforeExport;

            //SetExportOptions(richEditControl.Options.Export.Html);

            var doc = richEditControl.Document;
            var selRange = doc.GetSelectedRange();
            var clipboardFormat = richEditControl.GetClipboardFormat();

            try
            {
                var htmlData = doc.GetHtmlContent(selRange, true, richEditControl.GetTableTitle(),
                    richEditControl.Options.Export.Html);

                switch (clipboardFormat)
                {
                    case ClipboardFormat.All:
                    {
                        // Обрабатываем тег <span style="...">, добавляем отдельные теги, соответствующие значениям style
                        //htmlData = TransfromService.HtmlUtils.ProcessSpanTagStyleAttribute(htmlData, true);
                        htmlData = TransfromService.HtmlUtils.GetHtmlCleanValue(htmlData, null);
                        var htmlForClipboard = CF_HtmlHelper.GetHtmlClipboardFormat(htmlData);

                        var dataObject = new DataObject();
                        dataObject.SetData(OfficeDataFormats.Rtf,
                            doc.GetRtfText(selRange));
                        dataObject.SetData(OfficeDataFormats.UnicodeText,
                            doc.GetText(selRange));
                        dataObject.SetData(OfficeDataFormats.Html, htmlForClipboard);

                        //Clipboard.Clear();
                        Clipboard.SetDataObject(dataObject, true);
                        break;
                    }
                    case ClipboardFormat.Html:
                    {
                        // Очищаем HTML для последующей корректной обработки
                        htmlData = TransfromService.HtmlUtils.GetHtmlCleanValue(htmlData, null);
                        var htmlForClipboard = CF_HtmlHelper.GetHtmlClipboardFormat(htmlData);

                        var dataObject = new DataObject();
                        dataObject.SetData(OfficeDataFormats.Html, htmlForClipboard);
                        Clipboard.SetDataObject(dataObject, true);

                        break;
                    }
                }
            }
            catch
            {
                // ignored
            }
            //finally
            //{
            //    //richEditControl.BeforeExport -= OnBeforeExport;
            //}
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