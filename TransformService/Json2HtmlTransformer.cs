﻿using System;
using System.Linq;
using System.Text;
using TransformService.JsonData;

namespace TransformService
{
    public class Json2HtmlTransformer
    {
        public string Transform(string jsonData, Json2HtmlTransformParameters transformParams = null)
        {
            transformParams ??= new Json2HtmlTransformParameters();

            var htmlResult = string.Empty;

            if (string.IsNullOrEmpty(jsonData))
                return htmlResult;

            var jsonObject = JsonUtils.DeserializeObject(jsonData);

            if (jsonObject is TableJsonRoot { Content: { Table.Cells: not null } tableRoot })
            {
                htmlResult = CreateHtmlTable(tableRoot);
            }
            else if (jsonObject is TextJsonRoot textRoot)
            {
                htmlResult = CreateHtmlText(textRoot);
            }

            if (transformParams.MakeAllListsHierarchical)
                htmlResult = HtmlUtils.MakeAllListsHierarchical(htmlResult);

            return htmlResult;
        }

        private string CreateHtmlText(TextJsonRoot textRoot)
        {
            var htmlBuilder = new StringBuilder();

            htmlBuilder.Append(GetHtmlDocBegin());
            htmlBuilder.Append(GetHtmlBodyBegin());
            htmlBuilder.Append(textRoot.Content.Value);
            htmlBuilder.Append(GetHtmlDocEnd());

            return htmlBuilder.ToString();
        }

        private string CreateHtmlTable(TableRootContent tableRoot)
        {
            var result = string.Empty;
            var cells = tableRoot.Table.Cells.OrderBy(c => c.Y).ThenBy(c => c.X).ToList();

            if (cells.Any())
            {
                var numRows = cells.Max(c => c.Y) + 1;
                var numCols = cells.Max(c => c.X) + 1;

                var minRowIndex = cells.Min(c => c.Y);
                var minColIndex = cells.Min(c => c.X);


                var htmlBuilder = new StringBuilder();

                // Формирование стилей
                /*
                var headerStyleClassName = "cs162A16FE1";
                htmlBuilder.Append(GetHtmlDocBegin());
                htmlBuilder.Append("<style type=\"text/css\">");
                htmlBuilder.Append(
                    $".{headerStyleClassName}{{background-color:{HtmlUtils.CommonTableHeaderColor.HexValue};}}");
                htmlBuilder.Append("</style>");
                */

                htmlBuilder.Append(GetHtmlBodyBegin());
                
                // Формируем метаданные таблицы
                var tableMetadataAttrValue = string.Empty;
                var tableMetadata = new TableMetadata.TableMetadata(tableRoot.Title, tableRoot.Table.Widths, null);

                // Добавляем заголовок (наименование) таблицы
                if (!string.IsNullOrEmpty(tableMetadata.Title))
                    tableMetadataAttrValue = AddPrefixWhitespace(tableMetadataAttrValue) + $"{TableMetadata.TableMetadata.TitleAttributeName}=\"{tableMetadata.Title}\"";

                // Добавляем ширины столбцов таблицы
                if (tableRoot.Table.Widths != null && tableRoot.Table.Widths.Count != 0)
                    tableMetadataAttrValue = AddPrefixWhitespace(tableMetadataAttrValue) +
                                             $"{TableMetadata.TableMetadata.OriginalColumnWidthsAttributeName}=\"{TableMetadata.TableMetadataUtils.GetStringFromColumnWidths(tableMetadata.OriginalColumnWidths)}\"";

                htmlBuilder.Append($"<table{AddPrefixWhitespace(tableMetadataAttrValue) + tableMetadataAttrValue}>");

                //htmlBuilder.Append("<table" + (string.IsNullOrEmpty(tableRoot.Title)
                //    ? ">"
                //    : $" title='{tableRoot.Title}'>"));

                for (var row = minRowIndex; row < numRows; row++)
                {
                    htmlBuilder.Append("<tr>");

                    for (var col = minColIndex; col < numCols; col++)
                    {
                        var cell = cells.FirstOrDefault(c => c.X == col && c.Y == row);
                        if (cell != null)
                        {
                            // Обработка заголовка
                            var cellTag = cell.IsHeader == true ? "th" : "td";
                            var styleClass = (cell.IsHeader == true || (cell.IsAutoNumbered == true))
                                ? $" style=\"background-color:{HtmlUtils.CommonTableHeaderColor.HexValue};\""
                                // $" class=\"{headerStyleClassName}\""
                                : string.Empty;

                            string cellValue = null;
                            var contentValue = cell.Items.FirstOrDefault()?.Content?.Value ?? string.Empty;

                            // Обработка автонумерации
                            if (cell.IsAutoNumbered == true)
                            {
                                var autoNumberStr = contentValue
                                    .Replace("<p>", string.Empty, StringComparison.InvariantCultureIgnoreCase)
                                    .Replace("</p>", string.Empty, StringComparison.InvariantCultureIgnoreCase).Trim();

                                if (int.TryParse(autoNumberStr, out var autoNumber))
                                {
                                    cellValue = $"<ol start=\"{autoNumber}\"><li><span>&nbsp;</span></li></ol>";
                                }
                            }

                            if (string.IsNullOrEmpty(cellValue))
                                cellValue = contentValue;

                            var colSpan = cell.W > 1 ? $" colspan=\"{cell.W}\"" : string.Empty;
                            var rowSpan = cell.H > 1 ? $" rowspan=\"{cell.H}\"" : string.Empty;

                            htmlBuilder.AppendFormat("<{0}{1}{2}{3}>{4}</{0}>", cellTag, styleClass, rowSpan,
                                colSpan, cellValue);
                        }
                    }

                    htmlBuilder.Append("</tr>");
                }

                htmlBuilder.Append("</table>");
                htmlBuilder.Append(GetHtmlDocEnd());
                result = htmlBuilder.ToString();
            }

            return result;
        }

        private string AddPrefixWhitespace(string str) => string.IsNullOrEmpty(str) ? str : " " + str;

        private string GetHtmlDocBegin() =>
            "<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\">" +
            Environment.NewLine + "<html xmlns=\"http://www.w3.org/1999/xhtml\">";

        private string GetHtmlBodyBegin() => "<body>";

        private string GetHtmlDocEnd() => "</body>" + Environment.NewLine + "</html>";
    }

    public class Json2HtmlTransformParameters : ICloneable
    {
        ///// <summary>
        ///// Производить трансформацию через JSON (JSON -> HTML, HTML -> JSON, JSON -> HTML )
        ///// </summary>
        //public bool TransformViaJson { get; set; } = true;

        /// <summary>
        /// Признак трансформации всех списков из плоского представления в древовидное
        /// </summary>
        public virtual bool MakeAllListsHierarchical { get; set; } = false;

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}