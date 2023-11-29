﻿using DevExpress.XtraRichEdit.Export.Html;
using DevExpress.XtraRichEdit.Export;
using System.Linq;
using DevExpress.XtraRichEdit.API.Native;

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

        public static string GetHtmlContent(this Document document,
            TextRangeType textRangeType, string firstTableTitle, HtmlDocumentExporterOptions exportOptions)
        {
            var range = textRangeType == TextRangeType.Selection ? GetSelectedRange(document) : document.Range;

            return GetHtmlContent(document, range, firstTableTitle, exportOptions);
        }

        public static string GetHtmlContent(this Document document,
            DocumentRange range, string firstTableTitle, HtmlDocumentExporterOptions exportOptions)
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
                htmlData = SetFirstTableTitle(htmlData, firstTableTitle);

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
                    if (!string.IsNullOrEmpty(tableTitle))
                        table.SetAttributeValue("title", tableTitle);
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

            return null;
        }

        public static void SetCommonExportOptions(this HtmlDocumentExporterOptions exportHtml)
        {
            exportHtml.ExportRootTag = ExportRootTag.Html;
            exportHtml.CssPropertiesExportType = CssPropertiesExportType.Inline;
            exportHtml.EmbedImages = false;
        }

        public enum TextRangeType
        {
            All,
            Selection
        }
    }
}
