using DevExpress.Office.Utils;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.Commands;
using TransformService;
using TransformService.RichText;

namespace TextEditor.Editors.RichTextEdit.CustomCommands
{
    internal class CustomCopySelectionCommand : CopySelectionCommand
    {
        private readonly RichEditControl _richEditControl;
        private readonly IClipboardService _clipboardService;
        private readonly ITableMetadataManager _tableMetadataManager;

        public CustomCopySelectionCommand(RichEditControl control, IClipboardService clipboardService,
            ITableMetadataManager tableMetadataManager)
            : base(control)
        {
            _richEditControl = control;
            _clipboardService = clipboardService;
            _tableMetadataManager = tableMetadataManager;
        }

        protected override void ExecuteCore()
        {
            //if (Control is not RichEditControlEx richEditControl)
            //    return;

            //richEditControl.BeforeExport += OnBeforeExport;

            //SetExportOptions(richEditControl.Options.Export.Html);

            var doc = Control.Document;
            var selRange = doc.GetSelectedRange();
            var clipboardFormat = _clipboardService.GetClipboardFormat();

            try
            {
                var htmlData = doc.GetHtmlContent(selRange, _tableMetadataManager.GetTableMetadata(),
                    _richEditControl.Options.Export.Html);

                var transformParams = new Html2JsonTransformParameters
                {
                    MakeAllListsFlatten = false,
                    NeedDoubleTransformation = false
                };

                switch (clipboardFormat)
                {
                    case ClipboardFormat.All:
                    {
                        // Обрабатываем тег <span style="...">, добавляем отдельные теги, соответствующие значениям style
                        //htmlData = TransfromService.HtmlUtils.ProcessSpanTagStyleAttribute(htmlData, true);
                        htmlData = HtmlUtils.GetHtmlCleanValue(htmlData, transformParams);
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
                    case ClipboardFormat.Html: default:
                    {
                        // Очищаем HTML для последующей корректной обработки
                        htmlData = HtmlUtils.GetHtmlCleanValue(htmlData, transformParams);
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