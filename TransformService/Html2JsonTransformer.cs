using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using DevExpress.XtraRichEdit;
using HtmlAgilityPack;
using Newtonsoft.Json;
using TransformService.JsonData;
using TransformService.RichText;
using TransformService.TableMetadata;

namespace TransformService
{
    public class Html2JsonTransformer
    {
        private StyleClassesRegistry _styleClassesRegistry;

        //private HtmlNode _htmlRoot;

        public string Transform(string htmlData, Html2JsonTransformParameters transformParams)
        {
            if (string.IsNullOrEmpty(htmlData.Trim()))
                return null;

            var docNode = HtmlUtils.GetHtmlNodeFromText(htmlData);

            _styleClassesRegistry =
                new StyleClassesRegistry(docNode); // Создаем объект-обработчик стилей документа

            HtmlNode mainTable;
            TableMetadata.TableMetadata tableMetadata = null;

            // 1-й этап -> ищем тег table
            if (docNode.FirstChild.Name.Equals("tbody",
                    StringComparison.OrdinalIgnoreCase)) // Получаем тело таблицы, если первым не идет тег tbody
            {
                mainTable = docNode.FirstChild;
            }
            else
            {
                mainTable = docNode.SelectSingleNode("//table"); // Получаем первую таблицу

                if (mainTable != null)
                {
                    tableMetadata = TableMetadataUtils.GetTableMetadata(mainTable);
                    //mainTableTitle = mainTable.GetAttributeValue("title", null)?.Replace("&nbsp;", " ")
                    //    .Replace("&quot;", "\"");
                }
            }

            // 2-й этап -> ищем div таблицу, и если находим, то преобразуем ее в обычную table таблицу
            if (mainTable == null)
            {
                var tableFromDivResult = TryGetTableFromDivGrid(docNode);

                if (tableFromDivResult.HasValue)
                {
                    mainTable = tableFromDivResult.Value.tableNode;
                    tableMetadata = tableFromDivResult.Value.tableMetadata;
                }
            }

            // 3-й этап преобразуем найденный html в json
            var root = mainTable != null
                ? CreateJsonTable(mainTable, tableMetadata ?? new TableMetadata.TableMetadata(), transformParams)
                : CreateJsonText(docNode, transformParams);

            if (root == null)
                return null;

            var result = JsonUtils.SerializeObject(root,
                transformParams.NeedFormatJsonResult ? Formatting.Indented : Formatting.None);

            if (transformParams.NeedDoubleTransformation)
            {
                var doubleTransformParams = (Html2JsonTransformParameters)transformParams.Clone();
                doubleTransformParams.NeedDoubleTransformation = false;
                doubleTransformParams.MakeAllListsFlatten = false;
                doubleTransformParams.MultiLevelNumerationForFlattenList = false;

                result = ExecuteDoubleTransformation(result, tableMetadata, doubleTransformParams);
            }

            return result;
        }

        private JsonRootBase CreateJsonTable(HtmlNode mainTable, TableMetadata.TableMetadata tableMetadata,
            Html2JsonTransformParameters transformParams)
        {
            var rows = mainTable.SelectNodes(".//tr");

            if (rows == null)
                return null;

            var root = TableJsonRoot.GetRootInstanceForTable(tableMetadata);

            var y = 0;
            var colSpanMap = new Dictionary<int, int>();

            foreach (var row in rows)
            {
                // Проверяем, что строка не является невидимой
                if (row.Attributes["style"]?.Value == "display:none;")
                    continue;

                // Проверяем, содержится ли строка внутри тега <thead> с атрибутом style="display: none;"
                var thead = row.Ancestors("thead").FirstOrDefault();

                if (thead != null)
                    if (thead.Attributes["style"]?.Value == "display: none;" ||
                        thead.Attributes["class"]?.Value == "tableFloatingHeader")
                        continue;

                var cells = row.SelectNodes(".//td | .//th");
                if (cells == null)
                    continue;

                // Проверяем, что строка не вложена в другую таблицу
                if (HtmlUtils.IsNodeInsideNestedTable(mainTable, row))
                    continue;

                if (!HtmlUtils.IsRowVisible(row))
                    continue;

                var x = 0;

                foreach (var cell in cells)
                {
                    if (HtmlUtils.IsNodeInsideNestedTable(mainTable, cell))
                        continue;

                    if (!HtmlUtils.IsCellVisible(cell))
                        continue;

                    //string cellValue = GetCellValue(cell, cellValueFormat); // Формируем значение ячейки
                    var isHeaderCell = HtmlUtils.IsCellHeader(cell, transformParams.ProcessGreyBackgroundColorForCells,
                        _styleClassesRegistry); // Определяем является ли ячейка заголовком таблицы
                    string cellValue = null;

                    // Обработка автонумерации строк
                    var isAutoNumberedCell = false;
                    var autoNumber = 1;

                    if (transformParams.ProcessAutoNumberedRows)
                    {
                        var result = HtmlUtils.IsAutoNumberedRow(cell);

                        isAutoNumberedCell = result.Item1;
                        autoNumber = result.Item2;
                    }

                    if (isAutoNumberedCell)
                    {
                        cellValue = $"<p>{autoNumber.ToString()}</p>";
                        isHeaderCell = false;
                    }

                    // Формирование значения ячейки
                    if (string.IsNullOrEmpty(cellValue))
                        cellValue = HtmlUtils.GetNodeCleanValue(cell, isHeaderCell, transformParams,
                            _styleClassesRegistry);

                    var rowSpan = int.Parse(cell.GetAttributeValue("rowspan", "1"));
                    var colSpan = int.Parse(cell.GetAttributeValue("colspan", "1"));

                    // Вычисляем значение x с учетом объединенных ячеек
                    x = HtmlUtils.GetXWithColSpan(y, x, colSpanMap);

                    //var colSpan = 1;
                    //var rowSpan = 1;

                    //var colSpanAttribute = cell.Attributes["colspan"];
                    //if (colSpanAttribute != null && int.TryParse(colSpanAttribute.Value, out var colSpanValue))
                    //{
                    //    colSpan = colSpanValue;
                    //}

                    //var rowSpanAttribute = cell.Attributes["rowspan"];
                    //if (rowSpanAttribute != null && int.TryParse(rowSpanAttribute.Value, out var rowSpanValue))
                    //{
                    //    rowSpan = rowSpanValue;
                    //}

                    var cellData = new Cell
                    {
                        X = x, // Используем текущее значение x
                        Y = y,
                        W = colSpan,
                        H = rowSpan,
                        IsAutoNumbered = isAutoNumberedCell ? true : null,
                        IsHeader = isHeaderCell ? true : null,
                        Items = new List<Item>
                        {
                            new()
                            {
                                Uuid = Guid.NewGuid().ToString(),
                                Type = "TEXT",
                                Content = new ItemContent
                                {
                                    Value = cellValue
                                }
                            }
                        }
                    };

                    root.Content.Table.Cells.Add(cellData);

                    // Обновляем карту объединенных ячеек для текущей строки
                    for (var i = x; i < x + colSpan; i++)
                    {
                        colSpanMap[i] = y + rowSpan;
                    }

                    x += colSpan;
                }

                y++;
            }

            root.Content.Table.Cells.PostProcessCells();

            return root;
        }

        private JsonRootBase CreateJsonText(HtmlNode doc, Html2JsonTransformParameters transformParams)
        {
            var root = TextJsonRoot.GetRootInstanceForText();

            var bodyNode = doc.SelectSingleNode("//body");

            //var htmlToProcess = bodyNode is not null ? bodyNode.InnerHtml : doc.InnerHtml;
            var nodeToProcess = bodyNode ?? doc;
            //root.Content.Value = HtmlUtils.ProcessSpanTagStyleAttribute(innerHtml, transformParams.ProcessTextColor);
            root.Content.Value =
                HtmlUtils.GetNodeCleanValue(nodeToProcess, false, transformParams, _styleClassesRegistry);

            return root;
        }

        /// <summary>
        /// Выполнение двойной трансформации 1) JSON -> HTML, 2) HTML -> JSON
        /// </summary>
        /// <param name="jsonData"></param>
        /// <param name="tableMetadata"></param>
        /// <param name="transformParams"></param>
        /// <returns></returns>
        private string ExecuteDoubleTransformation(string jsonData, TableMetadata.TableMetadata tableMetadata,
            Html2JsonTransformParameters transformParams)
        {
            // Первая трансформация JSON -> HTML
            var htmlData = new Json2HtmlTransformer().Transform(jsonData);

            using (var server = new RichEditDocumentServer())
            {
                server.Options.Export.Html.SetCommonExportOptions();
                //server.HtmlText = null;
                server.HtmlText = htmlData;

                htmlData = server.Document.GetHtmlContent(RichTextUtils.TextRangeType.All, tableMetadata,
                    server.Options.Export.Html);
            }

            // Вторая трансформация HTML -> JSON
            return new Html2JsonTransformer().Transform(htmlData, transformParams);
        }

        private static (HtmlNode tableNode, TableMetadata.TableMetadata tableMetadata)? TryGetTableFromDivGrid(
            HtmlNode docNode)
        {
            var firstGridDiv = docNode.SelectSingleNode("//div[contains(@style, 'grid-area')]");

            if (firstGridDiv == null || firstGridDiv.GetAttributeValue("class", null)?.StartsWith("sc-") != true)
                return null; //new TableData{TableHtml = html};

            var containerDiv = firstGridDiv;
            for (var i = 0; i < 3; i++)
                if (containerDiv.ParentNode?.Name == "div")
                    containerDiv = containerDiv.ParentNode;

            var titleSpan = containerDiv.SelectSingleNode(".//span");
            var tableName = titleSpan?.InnerText.Trim();

            var tableNode = new HtmlNode(HtmlNodeType.Element, docNode.OwnerDocument, 0) { Name = "table" };
            var colSpanMap = new Dictionary<int, int>(); // Tracks last column impacted by colspan for each row
            var columnWidths = new SortedDictionary<int, int>();

            foreach (var div in containerDiv.SelectNodes(".//div[contains(@style, 'grid-area')]"))
            {
                var style = div.GetAttributeValue("style", "");
                var match = System.Text.RegularExpressions.Regex.Match(style,
                    @"grid-area:\s*(\d+)\s*/\s*(\d+)\s*/\s*span\s*(\d+)\s*/\s*span\s*(\d+);");

                if (!match.Success)
                    continue;

                if (!int.TryParse(match.Groups[1].Value, out var y)) y = 0;
                if (!int.TryParse(match.Groups[2].Value, out var x)) x = 0;
                if (!int.TryParse(match.Groups[3].Value, out var rowSpan)) rowSpan = 1;
                if (!int.TryParse(match.Groups[4].Value, out var colSpan)) colSpan = 1;

                var widthMatch = System.Text.RegularExpressions.Regex.Match(style, @"width:\s*([\d.]+)px;");
                if (widthMatch.Success && int.TryParse(widthMatch.Groups[1].Value, out var width))
                {
                    columnWidths.TryAdd(x, width);
                }

                while (tableNode.ChildNodes.Count < y)
                    tableNode.AppendChild(docNode.OwnerDocument.CreateElement("tr"));

                var row = tableNode.ChildNodes[y - 1];
                x = HtmlUtils.GetXWithColSpan(y, x, colSpanMap); // Вычисляем значение x с учетом объединенных ячеек

                //while (row.ChildNodes.Count < colStart - 1)
                //    row.AppendChild(doc.CreateElement("td"));

                var cell = docNode.OwnerDocument.CreateElement("td");
                cell.SetAttributeValue("rowspan", rowSpan.ToString());
                cell.SetAttributeValue("colspan", colSpan.ToString());
                cell.InnerHtml = div.InnerHtml;

                if (row.ChildNodes.Count >= x - 1)
                    row.ChildNodes.Insert(x - 1, cell);
                else
                    row.AppendChild(cell);

                // Обновляем карту объединенных ячеек для текущей строки
                for (var i = x; i < x + colSpan; i++)
                {
                    colSpanMap[i] = y + rowSpan;
                }
            }

            // Вставляем недостающие размеры колонок
            var maxColumnIndex = columnWidths.Keys.Any() ? columnWidths.Keys.Max() : 0;
            for (var i = 1; i <= maxColumnIndex; i++)
            {
                columnWidths.TryAdd(i, 200);
            }

            return (tableNode,
                new TableMetadata.TableMetadata(tableName,
                    columnWidths.Values.ToArray())); // { TableName = tableName, TableHtml = table.OuterHtml };
        }
    }

    public class Html2JsonTransformParameters : ICloneable
    {
        /// <summary>
        /// Формат значений ячеек для выходного файла - html или text 
        /// </summary>
        public virtual ValueFormat TargetFormat { get; set; } = ValueFormat.Html;

        /// <summary>
        /// Признак необходимости форматирования выходного результата
        /// </summary>
        public virtual bool NeedFormatJsonResult { get; set; } = false;

        /// <summary>
        /// Признак необходимости учета цвета текста
        /// </summary>
        public virtual bool ProcessTextColor { get; set; } = true;

        /// <summary>
        /// Признак необходимости замены табов пробелами, 1 таб - 4 пробела (используется, если текст ячейки представляет собой предварительно отформатированный код, например xml)
        /// </summary>
        public virtual bool ReplaceTabsBySpaces { get; set; } = false;

        /// <summary>
        /// Признак необходимости удаления форматирования у предварительно отформатированного кода перед его обработкой (удаление последовательности символов \r\n\t+)
        /// </summary>
        public virtual bool RemoveFormatting { get; set; } = true;

        /// <summary>
        /// Признак необходимости удаления жирного стиля для ячеек, являющимися заголовками
        /// </summary>
        public virtual bool RemoveBoldStyleForHeaderCells { get; set; } = true;

        /// <summary>
        /// Признак необходимости обработки автонумерации строк таблицы
        /// </summary>
        public virtual bool ProcessAutoNumberedRows { get; set; } = true;

        /// <summary>
        /// Признак необходимости обработки серого цвета для заливки ячеек (для установки признака заголовка)
        /// </summary>
        public virtual bool ProcessGreyBackgroundColorForCells { get; set; } = true;

        /// <summary>
        /// Признак необходимости двойного преобразования данных: из JSON в HTML и обратно (для корректной обработки тегов HTML, т.к. HTML редактора лучше воспринимается Сфера.Документы)
        /// </summary>
        public virtual bool NeedDoubleTransformation { get; set; } = true;

        /// <summary>
        /// Признак перевода всех списков в плоские 
        /// </summary>
        public virtual bool MakeAllListsFlatten { get; set; } = true;

        /// <summary>
        /// Многоуровневая нумерация для плоского списка
        /// </summary>
        public virtual bool MultiLevelNumerationForFlattenList { get; set; } = false;

        //public virtual bool CopyJsonToClipboardAfterTransformation { get; set; } = true;

        public object Clone()
        {
            return MemberwiseClone();
        }

        public enum ValueFormat
        {
            [Description("HTML")]
            Html = 0,
            [Description("Plain Text")]
            Text = 1
        }
    }
}