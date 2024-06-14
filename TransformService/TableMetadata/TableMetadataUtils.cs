using HtmlAgilityPack;
using System.Collections.Generic;
using System.Linq;

namespace TransformService.TableMetadata
{
    public static class TableMetadataUtils
    {
        /// <summary>
        /// Получить метаданные таблицы
        /// </summary>
        /// <param name="htmlData"></param>
        /// <returns></returns>
        public static TableMetadata GetFirstTableMetadata(string htmlData)
        {
            var docNode = HtmlUtils.GetHtmlNodeFromText(htmlData);

            if (docNode != null)
            {
                var tableNode = docNode.SelectSingleNode("//table");

                if (tableNode != null)
                    return GetTableMetadata(tableNode);
            }

            return new TableMetadata();
        }

        public static TableMetadata GetTableMetadata(HtmlNode tableNode)
        {
            var title = tableNode.GetAttributeValue(TableMetadata.TitleAttributeName, null)?.Replace("&nbsp;", " ")
                .Replace("&quot;", "\"");

            var originalColumnWidths =
                GetColumnWidthsFromString(tableNode.GetAttributeValue(TableMetadata.OriginalColumnWidthsAttributeName,
                    null));
            var actualColumnWidths =
                GetColumnWidthsFromString(tableNode.GetAttributeValue(TableMetadata.ActualColumnWidthsAttributeName,
                    null));

            var tableMetadata = new TableMetadata(title, originalColumnWidths, actualColumnWidths);

            return tableMetadata;
        }

        /// <summary>
        /// Записать метаданные в таблицу
        /// </summary>
        /// <param name="htmlData"></param>
        /// <param name="metadata"></param>
        /// <returns></returns>
        public static string SetFirstTableMetadata(string htmlData, TableMetadata metadata)
        {
            var docNode = HtmlUtils.GetHtmlNodeFromText(htmlData);
            var tableNode = docNode?.SelectSingleNode("//table");

            if (tableNode != null)
            {
                tableNode.WriteOrRemoveAttribute(TableMetadata.TitleAttributeName, metadata.Title);
                tableNode.WriteOrRemoveAttribute(TableMetadata.OriginalColumnWidthsAttributeName,
                    GetStringFromColumnWidths(metadata.OriginalColumnWidths));      
                tableNode.WriteOrRemoveAttribute(TableMetadata.ActualColumnWidthsAttributeName,
                    GetStringFromColumnWidths(metadata.ActualColumnWidths));
            }

            return docNode?.OuterHtml;
        }

        private static void WriteOrRemoveAttribute(this HtmlNode tableNode, string attributeName, string attributeValue)
        {
            if (!string.IsNullOrEmpty(attributeValue))
                tableNode.SetAttributeValue(attributeName, attributeValue);
            else
                tableNode.Attributes.Remove(attributeName);
        }

        /// <summary>
        /// Получение массива значений ширин столбцов из строки
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public static IEnumerable<int> GetColumnWidthsFromString(string values)
        {
            var colWidths = new List<int>();

            try
            {
                if (!string.IsNullOrEmpty(values))
                    colWidths.AddRange(values.Split(';').Select(int.Parse));
            }
            catch
            {
                //
            }

            return colWidths;
        }

        public static string GetStringFromColumnWidths(IEnumerable<int> columnWidths) => string.Join(";", columnWidths);

        public static IEnumerable<int> NormalizeColumnWidths(IEnumerable<int> columnWidths)
        {
            var normalizeColumnWidths = columnWidths as int[] ?? columnWidths.ToArray();

            if (!normalizeColumnWidths.Any())
                return normalizeColumnWidths;

            var maxColumnWidthSize = normalizeColumnWidths.Max();

            if (maxColumnWidthSize <= 0)
                return normalizeColumnWidths;

            return normalizeColumnWidths.Select(currentWidth =>
                currentWidth * TableMetadata.MaxColumnWidthSize / maxColumnWidthSize);
        }
    }
}
