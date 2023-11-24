using DevExpress.XtraRichEdit.Export.Html;
using DevExpress.XtraRichEdit.Export;
using DevExpress.XtraRichEdit;
using HtmlAgilityPack;
using static DevExpress.XtraPrinting.Native.ExportOptionsPropertiesNames;
using System;

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
            TextRange textRange, string firstTableTitle, HtmlDocumentExporterOptions exportOptions)
        {
            var htmlData = document.GetHtmlText(
                textRange == TextRange.Selection ? document.Selection : document.Range,
                //new LazyDataObject.HtmlClipboardUriProvider(),
                new CustomUriProvider(),
                exportOptions);

            // Записываем имя таблицы
            if (!string.IsNullOrEmpty(firstTableTitle))
                htmlData = SetFirstTableTitle(htmlData, firstTableTitle);

            return htmlData;
        }

        /// <summary>
        /// Записать заголовок для первой таблицы
        /// </summary>
        /// <param name="htmlData"></param>
        /// <param name="tableTitle"></param>
        /// <returns></returns>
        public static string SetFirstTableTitle(string htmlData, string tableTitle)
        {
            var docNode = Utils.GetHtmlNodeFromText(htmlData);

            if (docNode != null)
            {
                var table = docNode.SelectSingleNode("//table");

                if (table != null)
                {
                    if (tableTitle != null)
                        table.Attributes["title"].Value = tableTitle;
                    else
                        table.Attributes.Remove("title");

                    htmlData = docNode.OuterHtml;
                }
            }

            return htmlData;
        }

        /// <summary>
        /// Получить заголовок первой таблицы
        /// </summary>
        /// <param name="htmlData"></param>
        /// <returns></returns>
        public static string GetFirstTableTitle(string htmlData)
        {
            var docNode = Utils.GetHtmlNodeFromText(htmlData);

            if (docNode != null)
            {
                var table = docNode.SelectSingleNode("//table");

                if (table != null)
                    return table.GetAttributeValue("title", null);
            }

            return htmlData;
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
