using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using DevExpress.XtraRichEdit;
using HtmlAgilityPack;
using Newtonsoft.Json;
using TransfromService.JsonData;
using TransfromService.RichText;

namespace TransfromService
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
            string mainTableTitle = null;

            if (docNode.FirstChild.Name.Equals("tbody",
                    StringComparison.OrdinalIgnoreCase)) // Получаем тело таблицы, если первым не идет тег tbody
            {
                mainTable = docNode.FirstChild;
            }
            else
            {
                mainTable = docNode.SelectSingleNode("//table"); // Получаем первую таблицу

                if (mainTable != null)
                    mainTableTitle = mainTable.GetAttributeValue("title", null)?.Replace("&nbsp;", " ").Replace("&quot;", "\"");
            }

            JsonRootBase root;

            if (mainTable != null)
                root = CreateJsonTable(mainTable, mainTableTitle, transformParams);
            else
                root = CreateJsonText(docNode, transformParams);

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

                result = ExecuteDoubleTransformation(result, mainTableTitle, doubleTransformParams);
            }

            return result;
        }

        private JsonRootBase CreateJsonTable(HtmlNode mainTable, string mainTableTitle,
            Html2JsonTransformParameters transformParams)
        {
            var rows = mainTable.SelectNodes("//tr");

            if (rows == null)
                return null;

            var root = TableJsonRoot.GetRootInstanceForTable(mainTableTitle);

            var y = 0;
            var colspanMap = new Dictionary<int, int>();

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
                    var isCellHeader =
                        HtmlUtils.IsCellHeader(cell,
                            _styleClassesRegistry); // Определяем является ли ячейка заголовком таблицы
                    var cellValue =
                        HtmlUtils.GetNodeCleanValue(cell, isCellHeader, transformParams,
                            _styleClassesRegistry); // Формируем значение ячейки

                    var colspan = 1;
                    var rowspan = 1;

                    var colspanAttribute = cell.Attributes["colspan"];
                    if (colspanAttribute != null && int.TryParse(colspanAttribute.Value, out var colspanValue))
                    {
                        colspan = colspanValue;
                    }

                    var rowspanAttribute = cell.Attributes["rowspan"];
                    if (rowspanAttribute != null && int.TryParse(rowspanAttribute.Value, out var rowspanValue))
                    {
                        rowspan = rowspanValue;
                    }

                    // Вычисляем значение x с учетом объединенных ячеек
                    x = HtmlUtils.GetXWithColspan(y, x, colspanMap);

                    var cellData = new Cell
                    {
                        X = x, // Используем текущее значение x
                        Y = y,
                        W = colspan,
                        H = rowspan,
                        IsHeader = isCellHeader ? true : null,
                        Items = new List<Item>
                        {
                            new Item
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

                    // Обновляем карту объединенных ячеек для текущей строки
                    for (var i = x; i < x + colspan; i++)
                    {
                        colspanMap[i] = y + rowspan;
                    }

                    root.Content.Table.Cells.Add(cellData);
                    x += colspan;
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
        /// <param name="tableTitle"></param>
        /// <param name="transformParams"></param>
        /// <returns></returns>
        private string ExecuteDoubleTransformation(string jsonData, string tableTitle,
            Html2JsonTransformParameters transformParams)
        {
            // Первая трансформация JSON -> HTML
            var htmlData = new Json2HtmlTransformer().Transform(jsonData);

            using (var server = new RichEditDocumentServer())
            {
                server.Options.Export.Html.SetCommonExportOptions();
                //server.HtmlText = null;
                server.HtmlText = htmlData;

                htmlData = server.Document.GetHtmlContent(RichTextUtils.TextRangeType.All, true, tableTitle,
                    server.Options.Export.Html);
            }

            // Вторая трансформация HTML -> JSON
            return new Html2JsonTransformer().Transform(htmlData, transformParams);
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