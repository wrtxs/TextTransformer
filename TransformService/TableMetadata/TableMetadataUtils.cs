using HtmlAgilityPack;

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
            var colWidthsStr = tableNode.GetAttributeValue(TableMetadata.ColWidthsAttributeName, null);

            var tableMetadata = new TableMetadata(title, colWidthsStr);

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

            if (docNode != null)
            {
                var tableNode = docNode.SelectSingleNode("//table");

                if (tableNode != null)
                {
                    tableNode.WriteOrRemoveAttribute(TableMetadata.TitleAttributeName, metadata.Title);
                    tableNode.WriteOrRemoveAttribute(TableMetadata.ColWidthsAttributeName,
                        metadata.GetColumnWidthsString());
                }
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
    }
}
