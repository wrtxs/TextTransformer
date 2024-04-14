using DevExpress.XtraRichEdit.Export.Html;
using DevExpress.XtraRichEdit.Export;
using System.Linq;
using DevExpress.XtraRichEdit.API.Native;
using TransformService.TableMetadata;

namespace TransformService.RichText
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

        public static string GetHtmlContent(this Document document,
            TextRangeType textRangeType, TableMetadata.TableMetadata tableMetadata,  HtmlDocumentExporterOptions exportOptions)
        {
            var range = textRangeType == TextRangeType.Selection ? GetSelectedRange(document) : document.Range;

            return GetHtmlContent(document, range, tableMetadata,  exportOptions);
        }

        public static string GetHtmlContent(this Document document, DocumentRange range, TableMetadata.TableMetadata tableMetadata, HtmlDocumentExporterOptions exportOptions)
        {
            if (range == null)
                return null;

            try
            {
                range.BeginUpdateDocument();
                var htmlData = document.GetHtmlText(
                    range,
                    //new LazyDataObject.HtmlClipboardUriProvider(),
                    new CustomUriProvider(),
                    exportOptions);

                // Записываем или удаляем имя таблицы
                if (tableMetadata != null)
                    htmlData = TableMetadataUtils.SetFirstTableMetadata(htmlData, tableMetadata);

                return htmlData;
            }
            finally
            {
                range.EndUpdateDocument(document);
            }
        }

        public static DocumentRange GetSelectedRange(this Document document) =>
            document.Selections.Count > 1
                ? document.CreateRange(document.Selections[0].Start,
                    document.Selections.Sum(selection => selection.Length))
                : document.Selection;

        public static void SetCommonExportOptions(this HtmlDocumentExporterOptions exportHtml)
        {
            exportHtml.ExportRootTag = ExportRootTag.Html;
            exportHtml.CssPropertiesExportType = CssPropertiesExportType.Inline;
            exportHtml.EmbedImages = false;

            //exportHtml.TabMarker = "&nbsp;&nbsp;&nbsp;&nbsp;";
            //exportHtml.HtmlNumberingListExportFormat = HtmlNumberingListExportFormat.HtmlFormat;
            exportHtml.IgnoreHangingIndentOnNumberingList = false;
            //exportHtml.ExportListItemStyle = true;
        }

        public enum TextRangeType
        {
            All,
            Selection
        }
    }
}
