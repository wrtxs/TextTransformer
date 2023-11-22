using DevExpress.XtraRichEdit.Export.Html;
using DevExpress.XtraRichEdit.Export;
using DevExpress.XtraRichEdit;

namespace TransfromService.RichText
{
    public static class RichTextUtils
    {
        //public static string GetHtmlContent(this IRichEditControl richEditControl, TextRange textRange)
        //{
        //    return richEditControl.Document.GetHtmlContent(textRange, richEditControl.InnerDocumentServer.Options.Export.Html);
        //    //return richEditControl.Document.GetHtmlText(
        //    //    onlySelectionContent ? richEditControl.Document.Selection : richEditControl.Document.Range,
        //    //    //new LazyDataObject.HtmlClipboardUriProvider(),
        //    //    new CustomUriProvider(),
        //    //    richEditControl.Options.Export.Html);
        //}

        //public static string GetHtmlContent(this IRichEditDocumentServer richEditDocumentServer,
        //    TextRange textRange)
        //{
        //    return richEditDocumentServer.Document.GetHtmlContent(textRange,
        //        richEditDocumentServer.Options.Export.Html);
        //}

        public static string GetHtmlContent(this DevExpress.XtraRichEdit.API.Native.Document document,
            TextRange textRange, HtmlDocumentExporterOptions exportOptions)
        {
            return document.GetHtmlText(
                textRange == TextRange.Selection ? document.Selection : document.Range,
                //new LazyDataObject.HtmlClipboardUriProvider(),
                new CustomUriProvider(),
                exportOptions);
        }

        public static void SetCommonExportOptions(this HtmlDocumentExporterOptions exportHtml)
        {
            exportHtml.ExportRootTag = ExportRootTag.Html;
            exportHtml.CssPropertiesExportType = CssPropertiesExportType.Inline;
            exportHtml.EmbedImages = false;
        }

        public enum TextRange
        {
            All,
            Selection
        }
    }
}
