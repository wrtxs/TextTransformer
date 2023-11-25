using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
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

        private readonly ISet<string> _noProcessStyleClasses = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "auto-cursor-target",
            "external-link"
        };

        public string Transform(string htmlText, Html2JsonTransformParameters transformParams)
        {
            string result = null;

            if (string.IsNullOrEmpty(htmlText.Trim()))
                return result;

            var doc = new HtmlDocument();
            doc.LoadHtml(htmlText);

            string mainTableTitle = null;

            _styleClassesRegistry =
                new StyleClassesRegistry(doc.DocumentNode); // Создаем объект-обработчик стилей документа

            HtmlNode mainTable;

            if (doc.DocumentNode.FirstChild.Name.Equals("tbody",
                    StringComparison.OrdinalIgnoreCase)) // Получаем тело таблицы, если первым не идет тег tbody
            {
                mainTable = doc.DocumentNode.FirstChild;
            }
            else
            {
                mainTable = doc.DocumentNode.SelectSingleNode("//table"); // Получаем первую таблицу

                if (mainTable == null)
                    return result;

                mainTableTitle = mainTable.GetAttributeValue("title", null);
            }

            var rows = mainTable.SelectNodes("//tr");

            if (rows == null)
                return result;

            var root = Root.GetRootInstance(mainTableTitle);
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
                if (IsNodeInsideNestedTable(mainTable, row))
                    continue;

                if (!IsRowVisible(row))
                    continue;

                var x = 0;

                foreach (var cell in cells)
                {
                    if (IsNodeInsideNestedTable(mainTable, cell))
                        continue;

                    if (!IsCellVisible(cell))
                        continue;

                    //string cellValue = GetCellValue(cell, cellValueFormat); // Формируем значение ячейки
                    var isCellHeader = IsCellHeader(cell); // Определяем является ли ячейка заголовком таблицы
                    var cellValue = GetCellValue(cell, isCellHeader, transformParams); // Формируем значение ячейки

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
                    x = GetXWithColspan(y, x, colspanMap);

                    var cellData = new Cell
                    {
                        X = x, // Используем текущее значение x
                        Y = y,
                        W = colspan,
                        H = rowspan,
                        IsHeader = isCellHeader ? true : (bool?)null,
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

            PostProcessCells(root.Content.Table.Cells);

            result = JsonConvert.SerializeObject(root,
                transformParams.NeedFormatResult ? Formatting.Indented : Formatting.None);

            if (transformParams.NeedDoubleTransformation)
            {
                var doubleTransformParams = (Html2JsonTransformParameters)transformParams.Clone();
                doubleTransformParams.NeedDoubleTransformation = false;

                result = ExecuteDoubleTransformation(result, mainTableTitle, doubleTransformParams);
            }

            return result;
        }

        #region Постобработка полученных ячеек - корректировка высоты ячеек для учета возможного бага в html разметке

        /// <summary>
        /// Постобработка полученных ячеек для устранения багов с высотой ячеек в html разметке от DevExpress
        /// </summary>
        /// <param name="cells"></param>
        private void PostProcessCells(IReadOnlyList<Cell> cells)
        {
            foreach (var targetCell in cells.Where(c => c.H > 1)) // Получаем ячейки объединенные по высоте
            {
                if (cells.Any(c =>
                        c.Y == targetCell.Y + targetCell.H - 1 && c.W > 1 && targetCell.IntersectsByWidth(c)))
                    targetCell.H--;
            }
        }

        #endregion

        /// <summary>
        /// Проверка на то, что ячейка является вспомогательной, скрытой
        /// </summary>
        /// <param name="cell"></param>
        /// <returns></returns>
        private bool IsCellVisible(HtmlNode cell)
        {
            var styleValue = cell.GetAttributeValue("style", String.Empty);

            if (!string.IsNullOrEmpty(styleValue) &&
                styleValue.Contains("display: none;", StringComparison.OrdinalIgnoreCase))
                return false;

            return true;
        }

        private bool IsRowVisible(HtmlNode row)
        {
            var attributeValue = row.GetAttributeValue("contenteditable", string.Empty);

            if (!string.IsNullOrEmpty(attributeValue) &&
                attributeValue.Equals("false", StringComparison.OrdinalIgnoreCase))
                return false;

            return true;
        }

        /// <summary>
        /// Выполнение двойной трансформации 1) JSON -> HTML, 2) HTML -> JSON
        /// </summary>
        /// <param name="jsonData"></param>
        /// <param name="tableTitle"></param>
        /// <param name="transformParams"></param>
        /// <returns></returns>
        private string ExecuteDoubleTransformation(string jsonData, string tableTitle, Html2JsonTransformParameters transformParams)
        {
            // Первая трансформация JSON -> HTML
            var htmlData = new Json2HtmlTransformer().Transform(jsonData);

            using (var server = new RichEditDocumentServer())
            {
                server.Options.Export.Html.SetCommonExportOptions();
                server.HtmlText = htmlData;

                htmlData = server.Document.GetHtmlContent(RichTextUtils.TextRange.All, tableTitle,
                    server.Options.Export.Html);
            }

            // Вторая трансформация HTML -> JSON
            return new Html2JsonTransformer().Transform(htmlData, transformParams);
        }

        private string GetCellValue(HtmlNode cell, bool isCellHeader, Html2JsonTransformParameters transformParameters)
        {
            string cellValue;

            switch (transformParameters.CellValueFormat)
            {
                case CellValueFormat.Html:
                    //var cleanCell = (HtmlNode)cell.Clone();

                    //cleanCell = RemoveStyleAndClassAttributes(cleanCell); // Удалить у тегов атрибуты стиля и класса
                    //ReplaceClassByStyle(cleanCell); // Заменить атрибуты class на style

                    cellValue = cell.InnerHtml; //.Trim();

                    // Убираем символы форматирования html (используется для трансформации html, содержащего внутри себя фрагмент отформатированного код)
                    if (transformParameters.RemoveFormatting)
                        cellValue = Regex.Replace(cellValue, @"\r\n\t+", string.Empty);
                    
                    // Убираем символы неразрывных пробелов &nbsp; в начале и в конце html
                    cellValue = RemoveLeadingAndTrailingTokens(cellValue, "&nbsp;");//.Trim();

                    // Заменяем <pre> на <p>
                    cellValue = ReplaceTag(cellValue, "pre", "p");

                    // Заменяем <strong> на <b>
                    cellValue = ReplaceTag(cellValue, "strong", "b");

                    // Заменяем <code> на <span>
                    cellValue = ReplaceTagHavingAttributes(cellValue, "code", "span");

                    // Заменяем <span style="color: rgb(0,51,102);"> на <font color="rgb(0,51,102)"> с нужным значением цвета (в основном для Confluence таблиц)
                    cellValue = ProcessSpanTagStyleAttribute(cellValue, transformParameters.ProcessTextColor);

                    // Обрабатываем класс стилей для тега <span>, заменяем значения класса стиля на соответствующие теги (в основном для встренного редактора)
                    cellValue = ProcessSpanTagClassAttribute(cellValue, transformParameters.ProcessTextColor);

                    // Обрабатываем класс стилей для тега <p class="cs2654AE3A">, заменяем значения класса стиля на соответствующие теги (в основном для встроенного редактора)
                    // cellValue = ProcessPTagClassStyle(cellValue);

                    // Удаляем при необходимости теги <font color=""></font>
                    if (!transformParameters.ProcessTextColor)
                        cellValue = RemoveFontColorTags(cellValue);

                    // Удаляем div теги с классом "expand-control"
                    cellValue = ProcessTagsWithClass(cellValue, "div", "expand-control", TagProcessActionType.DeleteTagWithContent);
                    
                    // Удаляем div теги с классом "toolbar"
                    cellValue = ProcessTagsWithClass(cellValue, "div", "toolbar", TagProcessActionType.DeleteTagWithContent);

                    // Удаляем div теги с классом "container"
                    cellValue = ProcessTagsWithClass(cellValue, "div", "container", TagProcessActionType.DeleteTagWithoutContent);

                    // Удаляем ссылки Export to CSV
                    cellValue = ProcessTagsWithClass(cellValue, "a", "csvLink", TagProcessActionType.DeleteTagWithContent);

                    // Заменяем <div> на <p> внутри <td class="code"> (для корректной обработки кода)
                    cellValue = ReplaceTagsInsideTableCellWithCodeClass(cellValue, "div", "p");

                    // Убираем лишние переносы
                    cellValue = RemoveNewLines(cellValue);

                    //  Удаляем атрибуты style и class у всех тегов
                    cellValue = RemoveExcessAttributes(cellValue, new[] { "style", "class", "align" });

                    // Удаляем обрамляющие <p> и </p>, если они не содержат атрибутов
                    cellValue = RemovePTagsAtStartAndEnd(cellValue);

                    // Заменяем при необходимости Tab на неразрывные пробелы
                    if (transformParameters.ReplaceTabsBySpaces)
                        cellValue = cellValue.Replace("\t", "&nbsp;&nbsp;&nbsp;&nbsp;");

                    // Убираем фразу Развернуть...
                    // cellValue = cellValue.Replace("<span>Развернуть...</span>", String.Empty);

                    // Убираем ссылки вида <div><span><a href=\"#\">?</a></span></div>
                    //if (cellValue.Contains("<div><span><a href=\"#\">?</a></span></div>"))
                    //    cellValue = cellValue.Replace("<div><span><a href=\"#\">?</a></span></div>", String.Empty);

                    
                    // Убираем пустые теги <span> (формируются для пустых ячеек во встроенном редакторе)
                    cellValue = RemoveEmptyValueTags(cellValue);

                    // Обрабатываем заголовок
                    if (isCellHeader)
                    {
                        // Удаляем жирный стиль для шрифта
                        if (transformParameters.RemoveBoldStyleForHeaderCells)
                            cellValue = RemoveBoldTags(cellValue);

                        // Записываем пробел в пустые ячейки, чтобы избежать надписи "Заголовок столбца"
                        cellValue = Utils.WriteSpaceForEmptyCell(cellValue);
                    }

                    break;
                case CellValueFormat.Text:
                default:
                    cellValue = $"<p>{cell.InnerText}</p>";
                    break;
            }

            cellValue = cellValue.Replace("Export to CSV", string.Empty);//.Trim();
            cellValue = Regex.Replace(cellValue, @"\r\n\t+", string.Empty);

            return cellValue;
        }

        /// <summary>
        /// Удалить лишние переносы
        /// </summary>
        /// <param name="cellValue"></param>
        /// <returns></returns>
        private string RemoveNewLines(string cellValue) => cellValue.Replace("\r\n", string.Empty)
            .Replace("\n", string.Empty).Replace("\r", string.Empty);

        /// <summary>
        /// Удаление тегов <b> и </b>
        /// </summary>
        /// <param name="cellValue"></param>
        /// <returns></returns>
        private string RemoveBoldTags(string cellValue) => Regex.Replace(cellValue, "<[/]?b>", string.Empty);

        /// <summary>
        /// Удаление тегов, не имеющих значений
        /// </summary>
        /// <param name="cellValue"></param>
        /// <returns></returns>
        private string RemoveEmptyValueTags(string cellValue)
        {
            if (cellValue.Contains("<img", StringComparison.OrdinalIgnoreCase))  // Оставляем изображения как есть
                return cellValue;

            var cellNodeInnerText = Utils.GetHtmlNodeFromText(cellValue).InnerText.Trim();
            cellValue = cellValue.Trim();

            return cellValue.Equals("<span>&nbsp;</span>", StringComparison.OrdinalIgnoreCase) ||
                   cellNodeInnerText.Equals("&nbsp;", StringComparison.OrdinalIgnoreCase) ||
                   string.IsNullOrEmpty(cellNodeInnerText)
                ? string.Empty
                : cellValue;
            //cellValue = cellValue.Replace("<span>&nbsp;</span>", string.Empty);
            //cellValue = (cellNodeInnerText.Equals("&nbsp;", StringComparison.OrdinalIgnoreCase) ||
            //             string.IsNullOrEmpty(cellNodeInnerText))
            //    ? string.Empty
            //    : cellValue;
            //return cellValue;
        }

        static string RemoveLeadingAndTrailingTokens(string input, string tokenToRemove)
        {
            string pattern = $@"^\s*({tokenToRemove})+|\s*({tokenToRemove})+$";
            string cleanedString = Regex.Replace(input, pattern, "");

            return cleanedString;
        }

        private string RemoveFontColorTags(string html)
        {
            string pattern = @"<font[^>]*color=[""']([^""']*)[""'][^>]*>(.*?)</font>";
            string cleanedHtml = Regex.Replace(html, pattern, "$2");

            return cleanedHtml;
        }

        /// <summary>
        /// Обработать класс стиля
        /// Функция  для тега <span class=""> - заменить значения класса стиля на соответствующие теги
        /// </summary>
        /// <returns></returns>
        private string ProcessSpanTagClassAttribute(string html, bool processTextColor)
        {
            return Utils.ProcessSpanTagClassAttribute(html, processTextColor, _styleClassesRegistry);
            //var docNode = Utils.GetHtmlNodeFromText(html);

            //foreach (var spanTag in docNode.Descendants("span").ToList())
            //{
            //    ProcessSpanTagClassAttribute(spanTag, processTextColor);
            //}

            //return docNode.OuterHtml;
        }

        //private void ProcessSpanTagClassAttribute(HtmlNode spanTag, bool processTextColor)
        //{
        //    foreach (var className in Utils.GetClassAttributeValues(spanTag))
        //    {
        //        if (_styleClassesRegistry.Items.TryGetValue(className, out var styleClass))
        //        {
        //            var innerHtml = spanTag.InnerHtml;

        //            if (processTextColor && styleClass.ParametersDict.TryGetValue("color", out var colorValue))
        //                innerHtml = Utils.WrapTag(innerHtml, "font", "color", colorValue.Value);

        //            if (styleClass.ParametersDict.ContainsKey("font-weight") && styleClass
        //                    .ParametersDict["font-weight"].Value
        //                    .Equals("bold", StringComparison.OrdinalIgnoreCase))
        //                innerHtml = Utils.WrapTag(innerHtml, "b");

        //            if (styleClass.ParametersDict.ContainsKey("font-style") && styleClass
        //                    .ParametersDict["font-style"].Value
        //                    .Equals("italic", StringComparison.OrdinalIgnoreCase))
        //                innerHtml = Utils.WrapTag(innerHtml, "em");


        //            if (styleClass.ParametersDict.ContainsKey("text-decoration") && styleClass
        //                    .ParametersDict["text-decoration"].Value
        //                    .Equals("underline", StringComparison.OrdinalIgnoreCase))
        //                innerHtml = Utils.WrapTag(innerHtml, "u");


        //            spanTag.InnerHtml = innerHtml;
        //        }
        //    }
        //}

        /// <summary>
        /// Обработка тега <span style="...">, убираем атрибут style, заменяя его на:
        ///  - "color: rgb(0,128,0)" на <font color="rgb(0,128,0)"/>
        ///  - "font-weight: bolder" на <b/>
        /// </summary>
        /// <param name="html"></param>
        /// <param name="processTextColor"></param>
        /// <returns></returns>
        private string ProcessSpanTagStyleAttribute(string html, bool processTextColor)
        {
            return Utils.ProcessSpanTagStyleAttribute(html, processTextColor);
            //var docNode = Utils.GetHtmlNodeFromText(html);

            //foreach (var spanTag in docNode.Descendants("span").ToList())
            //{
            //    ProcessSpanTagStyleAttribute(spanTag, processTextColor);
            //}

            //return docNode.OuterHtml;
        }

        //private void ProcessSpanTagStyleAttribute(HtmlNode spanTag, bool processTextColor)
        //{
        //    var colorStyle = spanTag.GetAttributeValue("style", null);

        //    if (colorStyle != null)
        //    {
        //        var innerHtml = spanTag.InnerHtml;

        //        if (processTextColor)
        //        {
        //            var colorValueMatch = Regex.Match(colorStyle,
        //                @"(?<![-\w])color\s*:\s*(#(?:[0-9a-fA-F]{3}){1,2}|rgb\(\s*\d+\s*,\s*\d+\s*,\s*\d+\s*\))",
        //                RegexOptions.IgnoreCase);

        //            if (colorValueMatch.Success)
        //            {
        //                var colorValue = colorValueMatch.Groups[1].Value;
        //                colorValue = Utils.ConvertRgbStringToHexString(colorValue);
        //                innerHtml = Utils.WrapTag(innerHtml, "font", "color", colorValue);
        //            }
        //        }

        //        if (colorStyle.Contains("font-weight: bolder", StringComparison.OrdinalIgnoreCase))
        //        {
        //            innerHtml = Utils.WrapTag(innerHtml, "b");
        //        }

        //        spanTag.InnerHtml = innerHtml;
        //    }
        //}

        /// <summary>
        /// Найти заданные теги с заданным стилем и произвести над ними заданное действие
        /// </summary>
        /// <param name="html"></param>
        /// <param name="tag"></param>
        /// <param name="className"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        private string ProcessTagsWithClass(string html, string tag, string className, TagProcessActionType action)
        {
            var docNode = Utils.GetHtmlNodeFromText(html);

            var processNodes = docNode.Descendants(tag)
                .Where(node =>
                    node.Attributes.Contains("class") && node.Attributes["class"].Value
                        .Equals(className, StringComparison.OrdinalIgnoreCase))
                .ToList();

            foreach (var node in processNodes)
            {
                switch (action)
                {
                    case TagProcessActionType.DeleteTagWithContent:
                        node.Remove();
                        break;
                    case TagProcessActionType.DeleteTagWithoutContent:
                        node.ParentNode.RemoveChild(node, true);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(action), action, null);
                }
            }

            return docNode.OuterHtml;
        }

        private enum TagProcessActionType
        {
            DeleteTagWithContent,
            DeleteTagWithoutContent
        }

        private string ProcessPTagClassStyle(string cellValue)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(cellValue);

            foreach (var tag in doc.DocumentNode.Descendants("p").ToList())
            {
                foreach (var className in Utils.GetClassAttributeValues(tag))
                {
                    if (_styleClassesRegistry.Items.TryGetValue(className, out var styleClass))
                    {
                        //var innerHtml = tag.InnerHtml;

                        if (styleClass.ParametersDict.TryGetValue("text-align", out var alignValue)) // Выравнивание не имеет смысла, т.к. неможет быть применено в Сфера.Документы
                            tag.SetAttributeValue("align", alignValue.Value);
                    }
                }
            }


            return doc.DocumentNode.OuterHtml;
        }

        private string RemovePTagsAtStartAndEnd(string html)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            if (doc.DocumentNode.ChildNodes.Count == 1 &&
                doc.DocumentNode.FirstChild.Name.Equals("p", StringComparison.OrdinalIgnoreCase)) // Удаляем тег p в любом случае, т.к. выравнивание по абзацу отстутствует в Сфера.Документы
            {
                //if (doc.DocumentNode.FirstChild.Attributes.Count == 0 || cellIsHeader) // Не имеет смысла, т.к. выравнивание текста все равно не применяется в Сфера.Документы
                return doc.DocumentNode.FirstChild.InnerHtml;
            }

            return html;

            //string openTag = $"<{tagName}[^>]*>";
            //string closeTag = $"</{tagName}>";

            //if (html.StartsWith(openTag) && html.EndsWith(closeTag))
            //{
            //    int startIndex = html.IndexOf(openTag);
            //    int endIndex = html.LastIndexOf(closeTag) + closeTag.Length;

            //    return html.Remove(startIndex, endIndex - startIndex);
            //}
            //else
            //{
            //    return html;
            //}
        }

        //private string RemoveMatchingTags(string html, string tagName)
        //{
        //    HtmlDocument doc = new HtmlDocument();
        //    doc.LoadHtml(html);

        //    var nodesToRemove = doc.DocumentNode.SelectNodes($"//{tagName}");
        //    if (nodesToRemove != null)
        //    {
        //        foreach (var node in nodesToRemove)
        //        {
        //            node.ParentNode.RemoveChild(node);
        //        }
        //    }

        //    return doc.DocumentNode.InnerHtml;
        //}

        /// <summary>
        /// Удалить обрамляющий тег
        /// </summary>
        /// <param name="html"></param>
        /// <param name="tag"></param>
        /// <returns></returns>
        //private string RemoveRootNodeTag(string html, string tag)
        //{
        //    var result = html.Trim();

        //    var beginTag = $"<{tag}>";
        //    var endTag = $"</{tag}>";

        //    if (result.StartsWith(beginTag) && result.EndsWith(endTag))
        //    {
        //        if (result.StartsWith(beginTag))
        //            result = result.Substring(beginTag.Length); // Удаляем открывающий тег

        //        if (result.EndsWith(endTag))
        //            result = result.Substring(0, result.Length - endTag.Length); // Удаляем закрывающий тег

        //    }

        //    return result;
        //}

        /// <summary>
        /// Заменить все вхождения тега на другой тег
        /// </summary>
        /// <returns></returns>
        private string ReplaceTag(string html, string sourceTag, string targetTag) => html.Replace($"<{sourceTag}>", $"<{targetTag}>").Replace($"</{sourceTag}>", $"</{targetTag}>");

        /// <summary>
        /// Заменить один тег на другой с сохранением атрибутов
        /// </summary>
        /// <param name="html"></param>
        /// <param name="sourceTag"></param>
        /// <param name="targetTag"></param>
        /// <returns></returns>
        private string ReplaceTagHavingAttributes(string html, string sourceTag, string targetTag)
        {
            return ReplaceTagHavingAttributes(Utils.GetHtmlNodeFromText(html), sourceTag, targetTag).OuterHtml;
        }

        private HtmlNode ReplaceTagHavingAttributes(HtmlNode node, string sourceTag, string targetTag)
        {
            if (node.Name.Equals(sourceTag, StringComparison.OrdinalIgnoreCase))
            {
                node.Name = targetTag; // Заменяем имя тега
            }

            foreach (var childNode in node.ChildNodes)
            {
                ReplaceTagHavingAttributes(childNode, sourceTag, targetTag);
            }

            return node;
        }

        private string ReplaceTagsInsideTableCellWithCodeClass(string html, string sourceTag, string targetTag)
        {
            var docNode = Utils.GetHtmlNodeFromText(html);

            var tdNodes = docNode.Descendants("td")
                .Where(td => td.Attributes["class"] != null && td.Attributes["class"].Value.Equals("code", StringComparison.OrdinalIgnoreCase));

            foreach (var node in tdNodes)
            {
                ReplaceTagHavingAttributes(node, sourceTag, targetTag);
            }

            return docNode.OuterHtml;
        }

        //private string ReplaceTagWithAnotherInsideTable(string html, string sourceTag, string targetTag)
        //{
        //    var docNode = GetHtmlNodeFromText(html);

        //    var tables = docNode.Descendants("table").ToList();
        //    foreach (var table in tables)
        //    {
        //        var tableHtml = table.OuterHtml;
        //        var updatedTableHtml = ReplaceTagHavingAttributes(tableHtml, sourceTag, targetTag);
        //        table.ParentNode.InnerHtml = table.ParentNode.InnerHtml.Replace(tableHtml, updatedTableHtml);
        //    }

        //    return docNode.OuterHtml;
        //}

        
        /// <summary>
        /// Заменить у тегов атрибут class на атрибут style со значениями атрибута class 
        /// </summary>
        /// <param name="node"></param>
        private void ReplaceClassByStyle(HtmlNode node)
        {
            var elementsWithClass = node.Descendants().Where(e => e.Attributes.Contains("class"));

            foreach (var element in elementsWithClass)
            {
                var classValue = element.GetAttributeValue("class", string.Empty);

                // Разбираем значения class на отдельные классы
                var classNames = classValue.Split(' ');

                if (classNames.Length == 1 &&
                    _noProcessStyleClasses.Contains(classNames[0]))
                    continue;

                // Получаем значение атрибута style на основе классов
                var styleValue = GetStyleValueOnClassValues(classNames);

                element.SetAttributeValue("style", styleValue);
                element.Attributes.Remove("class");
            }

        }

        /// <summary>
        /// Формирование значения атрибута style на основе значений классов
        /// </summary>
        /// <param name="classNames"></param>
        /// <returns></returns>
        private string GetStyleValueOnClassValues(string[] classNames)
        {
            //PrepareStylesClassesDict();
            var result = string.Empty;

            foreach (var className in classNames)
            {
                if (_styleClassesRegistry.Items.TryGetValue(className, out var styleClass))
                {
                    result = styleClass.Parameters.Aggregate(result,
                        (current, styleClassParameter) => current + styleClassParameter);
                }
            }

            return result;
        }


        private string RemoveExcessAttributes(string html, string[] excessAttributes)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            var attributesSet = new HashSet<string>(excessAttributes, StringComparer.OrdinalIgnoreCase);

            foreach (var element in doc.DocumentNode.DescendantsAndSelf())
            {
                if (element.Name != "#text")
                {
                    foreach (var attribute in element.Attributes
                                 .Where(attr => attributesSet.Contains(attr.Name))
                                 .ToArray())
                    {
                        //if (attribute.Name.Equals("style", StringComparison.OrdinalIgnoreCase))
                        //    element.Attributes.Remove("style");

                        if (attribute.Name.Equals("class", StringComparison.OrdinalIgnoreCase))
                        {
                            var classNames = attribute.Value.Split(' ');

                            if (!(classNames.Length == 1 &&
                                  _noProcessStyleClasses.Contains(classNames[0])))
                                element.Attributes.Remove("class");
                        }
                        else element.Attributes.Remove(attribute);
                    }
                }
            }

            return doc.DocumentNode.OuterHtml;
        }

        //private HtmlNode RemoveStyleAndClassAttributes(HtmlNode node)
        //{
        //    foreach (var element in node.DescendantsAndSelf())
        //    {
        //        if (element.Name != "#text")
        //        {
        //            element.Attributes.Remove("style");
        //            element.Attributes.Remove("class");
        //        }
        //    }

        //    return node;

        //    //foreach (var attribute in node.Attributes.ToArray())
        //    //{
        //    //    if (attribute.Name.Equals("style", StringComparison.OrdinalIgnoreCase) ||
        //    //        attribute.Name.Equals("class", StringComparison.OrdinalIgnoreCase))
        //    //    {
        //    //        node.Attributes.Remove(attribute);
        //    //    }
        //    //}

        //    //foreach (var childNode in node.ChildNodes)
        //    //{
        //    //    RemoveStyleAndClassAttributes(childNode);
        //    //}

        //    //return node;
        //}

        private bool IsCellHeader(HtmlNode cell)
        {
            if (cell.Name.Equals("th", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            // Берем наименования классов у ячейки
            var classAttributeValue = cell.GetAttributeValue("class", null);
            if (!string.IsNullOrEmpty(classAttributeValue))
            {
                var classNames = classAttributeValue.Split(' ');

                foreach (var className in classNames)
                {
                    // Проверяем каждый класс на наличие background-color со значениями серых цветов
                    // + проверяем, что класс ячейки "highlight-grey"
                    if (HasBackgroundColorInStyleClass(className) ||
                        className.Equals("highlight-grey", StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                }
            }

            // Проверяем на наличие атрибута data-highlight-colour="grey"
            //var dhcAttributeValue = cell.GetAttributeValue("data-highlight-colour", null);

            //if (!string.IsNullOrEmpty(dhcAttributeValue) &&
            //    dhcAttributeValue.Equals("grey", StringComparison.OrdinalIgnoreCase))
            //    return true;

                // Проверяем стили ячейки
            var styleAttribute = cell.Attributes["style"];
            if (styleAttribute != null)
            {
                var styleValue = styleAttribute.Value.ToLower();
                // Разбиваем стиль на пары свойств и значений
                var stylePairs = styleValue.Split(';');

                foreach (var stylePair in stylePairs)
                {
                    // Разбиваем пару на свойство и значение
                    var keyValue = stylePair.Split(':');
                    if (keyValue.Length == 2)
                    {
                        var propertyName = keyValue[0].Trim().ToLower();
                        var propertyValue = keyValue[1].Trim().ToLower();
                        // Проверяем свойство background-color и его значение
                        if (propertyName.Equals("background-color", StringComparison.OrdinalIgnoreCase) &&
                            IsColorRefersToHeaderColors(
                                propertyValue)) // propertyValue.Equals(Utils.TableHeaderBackgroundColorHexValue, StringComparison.OrdinalIgnoreCase))
                        {
                            return true;
                        }
                    }
                }
            }


            var bgcolorAttribute = cell.Attributes["bgcolor"];

            if (bgcolorAttribute != null)
            {
                var bgСolorValue = bgcolorAttribute.Value.Trim();
                if (IsColorRefersToHeaderColors(bgСolorValue)) //bgcolorValue.Equals(Utils.TableHeaderBackgroundColorHexValue, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Признак того, что цвет относится к цветам заголовка
        /// </summary>
        /// <param name="colorHexValue"></param>
        /// <returns></returns>
        private bool IsColorRefersToHeaderColors(string colorHexValue)
        {
            return Utils.TableHeaderColors.Any(color =>
                color.HexValue.Equals(colorHexValue, StringComparison.OrdinalIgnoreCase));
        }

        private bool HasBackgroundColorInStyleClass(string className)
        {
            //PrepareStylesClassesDict();

            if (_styleClassesRegistry.Items.TryGetValue(className, out var styleClass))
            {
                if (styleClass.ParametersDict.TryGetValue("background-color", out var bgColorParameter))
                {
                    if (IsColorRefersToHeaderColors(bgColorParameter.Value))
                        return true;
                }
            }

            return false;
        }

        //private void PrepareStylesClassesDict()
        //{
        //    // Получаем все теги <style>
        //    if (_styleClasses != null)
        //        return;

        //    _styleClasses = new Dictionary<string, StyleClass>();

        //    var styleNodes = _htmlRoot.Descendants("style");

        //    // Проходим по всем тегам <style>
        //    foreach (var styleNode in styleNodes)
        //    {
        //        var styleContent = styleNode.InnerText;

        //        // Удаляем комментарии из кода
        //        styleContent = Regex.Replace(styleContent, @"\/\*(.*?)\*\/", "", RegexOptions.Singleline);

        //        var classRegex = new Regex(@"\.(.*?){(.*?)}", RegexOptions.Singleline);
        //        var matches = classRegex.Matches(styleContent);

        //        // Проходим по всем классам стилей внутри конкретного тега <style>
        //        foreach (Match match in matches)
        //        {
        //            var styleClassName = match.Groups[1].Value.Trim(); // Например: 'csAD7A2888'
        //            var styleClassRawValue =
        //                match.Groups[2].Value
        //                    .Trim(); // Например: '{text-align:left;text-indent:0pt;margin:12pt 0pt 12pt 0pt}'
        //            var styleClassParams = new List<ClassStyleParameter>();
        //            var styleClassParamsDict = new Dictionary<string, ClassStyleParameter>();

        //            var pairValues = styleClassRawValue.Split(';');
        //            foreach (var pairValue in pairValues)
        //            {
        //                var keyValue = pairValue.Split(':');
        //                if (keyValue.Length == 2)
        //                {
        //                    var styleClassParamName = keyValue[0].Trim().ToLower();
        //                    var styleClassParamValue = keyValue[1].Trim().ToLower();

        //                    var classStyleParameter =
        //                        new ClassStyleParameter(styleClassParamName, styleClassParamValue);

        //                    if (!styleClassParamsDict.ContainsKey(styleClassParamName))
        //                    {
        //                        styleClassParams.Add(classStyleParameter);
        //                        styleClassParamsDict.Add(styleClassParamName, classStyleParameter);
        //                    }
        //                    else
        //                    {
        //                        styleClassParams.Remove(styleClassParams.Find(_ => _.Name == styleClassParamName));
        //                        styleClassParams.Add(classStyleParameter);
        //                        styleClassParamsDict[styleClassParamName] = classStyleParameter;
        //                    }
        //                }
        //            }

        //            _styleClasses.Add(styleClassName,
        //                new StyleClass(styleClassName, styleClassRawValue, styleClassParams, styleClassParamsDict));
        //        }
        //    }
        //}

        
        // Метод для определения значения x с учетом объединенных ячеек
        private int GetXWithColspan(int currentY, int currentX, IReadOnlyDictionary<int, int> colspanMap)
        {
            var x = currentX;

            while (colspanMap.TryGetValue(x, out var y) && y > currentY)
            {
                x++;
            }

            return x;
        }

        private bool IsNodeInsideNestedTable(HtmlNode mainTable, HtmlNode node)
        {
            var parentNode = node.ParentNode;

            while (parentNode != null)
            {
                if (parentNode.OriginalName.Equals("table", StringComparison.OrdinalIgnoreCase))
                {
                    if (!parentNode.Equals(mainTable))
                        return true;
                }

                parentNode = parentNode.ParentNode;
            }

            return false;
        }
    }

    public class Html2JsonTransformParameters : ICloneable
    {
        /// <summary>
        /// Формат значений ячеек для выходного файла - html или text 
        /// </summary>
        public CellValueFormat CellValueFormat { get; set; } = CellValueFormat.Html;
        
        /// <summary>
        /// Признак необходимости форматирования выходного результата
        /// </summary>
        public bool NeedFormatResult { get; set; } = false;

        /// <summary>
        /// Признак необходимости учета цвета текста
        /// </summary>
        public bool ProcessTextColor { get; set; } = true;

        /// <summary>
        /// Признак необходимости замены табов пробелами, 1 таб - 4 пробела (используется, если текст ячейки представляет собой предварительно отформатированный код, например xml)
        /// </summary>
        public bool ReplaceTabsBySpaces { get; set; } = false;
        
        /// <summary>
        /// Признак необходимости удаления форматирования у предварительно отформатированного кода перед его обработкой (удаление последовательности символов \r\n\t+)
        /// </summary>
        public bool RemoveFormatting { get; set; } = true;
        
        /// <summary>
        /// Признак необходимости удаления жирного стиля для ячеек, являющимися заголовками
        /// </summary>
        public bool RemoveBoldStyleForHeaderCells { get; set; } = true;

        /// <summary>
        /// Признак необходимости двойного преобразования данных: из JSON в HTML и обратно (для корректной обработки тегов HTML, т.к. HTML редактора лучше воспринимается Сфера.Документы)
        /// </summary>
        public bool NeedDoubleTransformation { get; set; } = true;

        public object Clone()
        {
            return MemberwiseClone();
        }
    }

    public enum CellValueFormat
    {
        Html,
        Text
    }

    public class CellBackgroundColor
    {
        public Color Value { get; }
        public string HexValue { get; }
        public int ArgbValue { get; }

        public CellBackgroundColor(int red, int green, int blue)
        {
            Value = Color.FromArgb(red, green, blue);
            HexValue = Value.ToHex();
            ArgbValue = Value.ToArgb();
        }
    }
}