using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using DevExpress.Utils.Text;
using TransfromService.JsonData;

namespace TransfromService
{
    public static class HtmlUtils
    {
        private static readonly ISet<string> NoProcessStyleClasses =
            new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "auto-cursor-target",
                "external-link"
            };
        //public static Color TableHeaderBackgroundColor { get; }
        //public static string TableHeaderBackgroundColorHexValue { get; }

        //public static int TableHeaderBackgroundColorArgbValue { get; }

        public static CellBackgroundColor CommonTableHeaderColor { get; }

        public static IReadOnlyList<CellBackgroundColor> TableHeaderColors { get; }

        static HtmlUtils()
        {
            CommonTableHeaderColor = new CellBackgroundColor(216, 216, 216);

            TableHeaderColors = new List<CellBackgroundColor>
            {
                CommonTableHeaderColor,
                new(244, 245, 247), // Цвет ячейки-заголовка Confluence 
                new(242, 242, 242),
                new(191, 191, 191),
                new(165, 165, 165),
                new(127, 127, 127)
            };


            //TableHeaderBackgroundColor = Color.FromArgb(216, 216, 216);
            //TableHeaderBackgroundColorHexValue = TableHeaderBackgroundColor.ToHex();
            //TableHeaderBackgroundColorArgbValue = TableHeaderBackgroundColor.ToArgb();
        }

        public static string GetHtmlCleanValue(string htmlData, Html2JsonTransformParameters transformParams)
        {
            transformParams ??= new Html2JsonTransformParameters();
            var node = GetHtmlNodeFromText(htmlData);

            return GetNodeCleanValue(node, false, transformParams, null);
        }

        public static string GetNodeCleanValue(HtmlNode node, bool isCellHeader,
            Html2JsonTransformParameters transformParams, StyleClassesRegistry styleClassesRegistry)
        {
            string nodeValue;

            switch (transformParams.TargetFormat)
            {
                case Html2JsonTransformParameters.ValueFormat.Html:
                {
                    //var cleanCell = (HtmlNode)cell.Clone();

                    //cleanCell = RemoveStyleAndClassAttributes(cleanCell); // Удалить у тегов атрибуты стиля и класса
                    //ReplaceClassByStyle(cleanCell); // Заменить атрибуты class на style

                    nodeValue = node.InnerHtml; //.Trim();

                    // Убираем пустые теги вида <a name="..."></a>
                    nodeValue = RemoveEmptyATags(nodeValue);

                    // Убираем символы форматирования html (значение false используется для html, содержащего внутри себя фрагмент отформатированного кода)
                    if (transformParams.RemoveFormatting)
                        nodeValue = RemoveFormatting(nodeValue);

                    // Переводим при необходимости все списки в плоские
                    if (transformParams.MakeAllListsFlatten)
                        nodeValue = MakeAllListsFlatten(nodeValue, transformParams.MultiLevelNumerationForFlattenList);

                    // Убираем символы неразрывных пробелов &nbsp; в начале и в конце html
                    nodeValue = RemoveLeadingAndTrailingTokens(nodeValue, "&nbsp;"); //.Trim();

                    // Заменяем <pre> на <p>
                    nodeValue = ReplaceTag(nodeValue, "pre", "p");

                    // Заменяем <strong> на <b>
                    nodeValue = ReplaceTag(nodeValue, "strong", "b");

                    // Заменяем <code> на <span>
                    nodeValue = ReplaceTagHavingAttributes(nodeValue, "code", "span");

                    // Для <span style="color: rgb(0,51,102);"> добавляем соответствующие теги (в частности, <font color="rgb(0,51,102)"> с нужным значением цвета, в основном для Confluence таблиц)
                    nodeValue = ProcessSpanTagStyleAttribute(nodeValue, transformParams.ProcessTextColor);

                    // Обрабатываем класс стилей для тега <span>, заменяем значения класса стиля на соответствующие теги (в основном для встренного редактора)
                    nodeValue = ProcessSpanTagClassAttribute(nodeValue, transformParams.ProcessTextColor,
                        styleClassesRegistry);

                    // Обрабатываем класс стилей для тега <p class="cs2654AE3A">, заменяем значения класса стиля на соответствующие теги (в основном для встроенного редактора)
                    // cellValue = ProcessPTagClassStyle(cellValue);

                    // Удаляем при необходимости теги <font color=""></font>
                    if (!transformParams.ProcessTextColor)
                        nodeValue = RemoveFontColorTags(nodeValue);

                    // Удаляем div теги с классом "expand-control"
                    nodeValue = ProcessTagsWithClass(nodeValue, "div", "expand-control",
                        TagProcessActionType.DeleteTagWithContent);

                    // Удаляем div теги с классом "toolbar"
                    nodeValue = ProcessTagsWithClass(nodeValue, "div", "toolbar",
                        TagProcessActionType.DeleteTagWithContent);

                    // Удаляем div теги с классом "container"
                    nodeValue = ProcessTagsWithClass(nodeValue, "div", "container",
                        TagProcessActionType.DeleteTagWithoutContent);

                    // Удаляем span тег с классом "collapse-source expand-control"
                    nodeValue = ProcessTagsWithClass(nodeValue, "span", "collapse-source expand-control",
                        TagProcessActionType.DeleteTagWithContent);

                    // Удаляем ссылки Export to CSV
                    nodeValue = ProcessTagsWithClass(nodeValue, "a", "csvLink",
                        TagProcessActionType.DeleteTagWithContent);

                    // Заменяем <div> на <p> внутри <td class="code"> (для корректной обработки кода)
                    nodeValue = ReplaceTagsInsideTableCellWithCodeClass(nodeValue, "div", "p");

                    // Убираем лишние переносы
                    nodeValue = RemoveNewLines(nodeValue);

                    //  Удаляем атрибуты style и class у всех тегов
                    nodeValue = RemoveExcessAttributes(nodeValue, new[] { "style", "class", "align" });

                    // Удаляем обрамляющие <p> и </p>, если они не содержат атрибутов
                    nodeValue = RemovePTagsAtStartAndEnd(nodeValue);

                    // Заменяем при необходимости Tab на неразрывные пробелы
                    if (transformParams.ReplaceTabsBySpaces)
                        nodeValue = nodeValue.Replace("\t", "&nbsp;&nbsp;&nbsp;&nbsp;");

                    // Убираем фразу Развернуть...
                    // cellValue = cellValue.Replace("<span>Развернуть...</span>", String.Empty);

                    // Убираем ссылки вида <div><span><a href=\"#\">?</a></span></div>
                    //if (cellValue.Contains("<div><span><a href=\"#\">?</a></span></div>"))
                    //    cellValue = cellValue.Replace("<div><span><a href=\"#\">?</a></span></div>", String.Empty);

                    // Убираем пустые теги <span> (формируются для пустых ячеек во встроенном редакторе)
                    nodeValue = RemoveEmptyValueTags(nodeValue);

                    // Обрабатываем заголовок
                    if (isCellHeader)
                    {
                        // Удаляем жирный стиль для шрифта
                        if (transformParams.RemoveBoldStyleForHeaderCells)
                            nodeValue = RemoveBoldTags(nodeValue);

                        // Записываем пробел в пустые ячейки, чтобы избежать надписи "Заголовок столбца"
                        nodeValue = HtmlUtils.WriteSpaceForEmptyCell(nodeValue);
                    }

                    // Стоит пересмотреть, возможно нужно будет убрать 
                    if (!transformParams.RemoveFormatting && transformParams.ReplaceTabsBySpaces)
                        nodeValue = RemoveFormatting(nodeValue);

                    nodeValue = RemoveExcessPhrases(nodeValue);

                    break;
                }
                case Html2JsonTransformParameters.ValueFormat.Text:
                default:
                {
                    nodeValue = $"<p>{node.InnerText}</p>";
                    nodeValue = RemoveFormatting(nodeValue);
                    nodeValue = RemoveExcessPhrases(nodeValue);

                    break;
                }
            }

            //nodeValue = nodeValue.Replace("Export to CSV", string.Empty); //.Trim();

            return nodeValue;
        }

        #region Обработка списков

        /// <summary>
        /// Перевод всех списков узла в иерархические
        /// </summary>
        public static string MakeAllListsHierarchical(string htmlData)
        {
            //htmlData = HtmlEntity.DeEntitize(htmlData);
            var node = GetHtmlNodeFromText(htmlData);

            //var lists = node.SelectNodes("//ul[not(parent::*/ul)] | //ol[not(parent::*/ol)]");

            foreach (var parentList in node.GetTopLevelLists())
            {
                var listWasChanged = MakeListHierarchical(parentList);

                if (listWasChanged &&
                    !parentList.Name.Equals("ol",
                        StringComparison.OrdinalIgnoreCase)) // Изменяем список на нумерованный
                    parentList.Name = "ol";
            }

            return node.OuterHtml;
        }

        /// <summary>
        /// Перевод списка в иерархический
        /// </summary>
        static bool MakeListHierarchical(HtmlNode parentList)
        {
            var listWasChanged = false;
            var listItems = parentList.Descendants("li").ToList();
            var listItemsDict = new Dictionary<HtmlNode, ListItemInfo>();

            for (var i = 0; i < listItems.Count; i++)
            {
                var listItem = listItems[i];
                
                var listItemInfo = GetListItemNumberInfo(listItem, listItemsDict);

                if (i == 0) // Устанавливаем значение атрибута start для списка
                {
                    var listStartNum = listItemInfo.GetIntNumber();

                    if (listStartNum != null)
                        parentList.SetAttributeValue("start", listStartNum.Value.ToString());
                }

                var parentListItemNumber = listItemInfo.ParentNumber;

                while (!string.IsNullOrEmpty(parentListItemNumber))
                {
                    // Ищем родительский элемент по номеру (части номера дочернего элемента)
                    var parentListItem = listItems.FirstOrDefault(e =>
                        GetListItemNumberInfo(e, listItemsDict).Number
                            .Equals(parentListItemNumber,
                                StringComparison.OrdinalIgnoreCase));

                    if (parentListItem != null)
                    {
                        // Ищем список внутри родительского элемента, создаем список при его отсутствии
                        var nestedList = parentListItem.GetTopLevelLists().FirstOrDefault();

                        if (nestedList == null)
                        {
                            nestedList = parentList.OwnerDocument.CreateElement("ol");
                            parentListItem.AppendChild(nestedList);
                        }

                        if (!nestedList.Name.Equals("ol",
                                StringComparison.OrdinalIgnoreCase)) // Изменяем список на нумерованный
                            parentList.Name = "ol";

                        //nestedList.AppendChild(listItem);
                        //listItem.Remove();

                        // Убираем номер из названия
                        listItem.InnerHtml = listItem.InnerHtml =
                            listItem.InnerHtml.ReplaceFirstOccurrence(listItemInfo.FullNumber, string.Empty);

                        listItemInfo.WasNumberRemovedFromName = true;

                        // Переносим элемент списка во вложенный список
                        nestedList.MoveChild(listItem);

                        // Удаляем исходный элемент
                        // listItem.ParentNode.RemoveChild(listItem);

                        listWasChanged = true;

                        break;
                    }

                    parentListItemNumber = GetParentNumberFromItemListNumber(parentListItemNumber);
                }
            }

            // Удаляем номер из наименований элементов списка
            if (listWasChanged)
            {

                foreach (var listItem in listItems)
                {
                    var listItemInfo = GetListItemNumberInfo(listItem, listItemsDict);

                    //var pli = parentList.Descendants("li").FirstOrDefault(node => node.InnerHtml.StartsWith(listItemInfo.FullNumber, StringComparison.OrdinalIgnoreCase));

                    if (!listItemInfo.WasNumberRemovedFromName && !string.IsNullOrEmpty(listItemInfo.FullNumber))
                    {
                        //parentList.InnerHtml =
                        //    parentList.InnerHtml.ReplaceFirstOccurrence(listItemInfo.FullNumber, string.Empty);

                        listItem.InnerHtml =
                            listItem.InnerHtml.ReplaceFirstOccurrence(listItemInfo.FullNumber, string.Empty);
                    }
                    //if (listItemInfo.ParentList != null)
                    //    listItemInfo.ParentList.MoveChild(listItem);
                }
            }

            return listWasChanged;
        }

        /// <summary>
        /// Перевод всех списков узла в плоские
        /// </summary>
        /// <param name="htmlData"></param>
        /// <param name="multiLevelNumeration"></param>
        /// <returns></returns>
        private static string MakeAllListsFlatten(string htmlData, bool multiLevelNumeration)
        {
            var node = GetHtmlNodeFromText(htmlData);

            //var lists = node.SelectNodes("//ul[not(parent::*/ul)] | //ol[not(parent::*/ol)]");

            foreach (var parentList in node.GetTopLevelLists())
            {
                var listWasChanged = MakeListFlatten(parentList, multiLevelNumeration, string.Empty,
                    GetListItemStartNumber(parentList), null);

                if (listWasChanged && multiLevelNumeration &&
                    !parentList.Name.Equals("ul")) // Изменяем список на ненумерованный
                    parentList.Name = "ul";
            }

            return node.OuterHtml;
        }

        /// <summary>
        /// Перевод списка в плоский
        /// </summary>
        static bool MakeListFlatten(HtmlNode parentList, bool multiLevelNumeration, string parentNumberPath,
            int itemListStartNumber, IList<ListItemInfo> innerListItemsInfo)
        {
            var listWasChanged = false;
            var itemNumber = itemListStartNumber;
            var isTopLevelList = (innerListItemsInfo == null);

            // Словарь с информацией об элементах списка: key -> информация об элементе списка самого верхнего уровня, value -> список с информацией об элементах дочерних списков, по отношению к элементу списка самого верхнего уровня
            var listItemInfos = isTopLevelList ? new List<ListItemInfo>() : null;

            var listItems = parentList.GetTopLevelItems().ToList(); // Получаем элементы списка: li, ol, ul
            
            // Проходим по всем элементам списка
            foreach (var listItem in listItems) 
            {
                innerListItemsInfo ??= new List<ListItemInfo>();

                // Запоминаем позицию для вставки склонированного элемента в оригинальный список
                var listItemPosition = isTopLevelList ? 0 : innerListItemsInfo.Count;
                List<HtmlNode> itemLists;
                var listItemIsList = listItem.IsList(); // Признак того, что элемент списка является списком (ol или ul)

                if (listItemIsList) // Элемент списка - является списком (ol или ul)
                {
                    itemLists = new List<HtmlNode>() { listItem };

                    if (itemNumber > 0 &&
                        listItems.Any(node => node.Name.Equals("li", StringComparison.OrdinalIgnoreCase)))
                        itemNumber--;
                }
                else // Элемент списка - li
                {
                    itemLists = listItem.GetTopLevelLists().ToList();
                }

                // Опредляем порядковый номер элемента списка
                var listItemInfo = new ListItemInfo
                {
                    ListItem = listItem,
                    Number = multiLevelNumeration ? GetListItemNumber(parentNumberPath, itemNumber) : string.Empty
                };

                // Проходим по всем вложенным спискам текущего элемента списка
                foreach (var nestedList in itemLists)
                {
                    var listItemStartNumber = nestedList.GetListItemStartNumber();

                    MakeListFlatten(nestedList, multiLevelNumeration,
                        GetListItemNumber(parentNumberPath, itemNumber), listItemStartNumber, innerListItemsInfo);

                    //var clone = nestedList.Clone();
                    //parentList.ParentNode.InsertBefore(clone, parentList);

                    nestedList.Remove(); // Удаляем вложенный список после обработки
                }

                if (!isTopLevelList) // Элемент вложенного списка
                {
                    // Копируем и выносим элемент из вложенного списка перед вложенным списком, после чего удаляем элемент
                    listItemInfo.ListItem = listItem.Clone();
                    innerListItemsInfo.Insert(listItemPosition, listItemInfo);

                    // Удаляем оригинальный элемент из вложенного списка
                    listItem.Remove();
                }
                else // Элемент верхнеуровневого списка
                {
                    if (innerListItemsInfo.Count > 0)
                    {
                        listWasChanged = true;
                        listItemInfo.ChildItemInfos.AddRange(innerListItemsInfo);
                    }

                    listItemInfos.Add(listItemInfo);
                }

                itemNumber++;

                if (isTopLevelList)
                    innerListItemsInfo = null;
            }

            if (isTopLevelList && listWasChanged)
            {
                for (var i = 0; i < listItemInfos.Count; i++)
                {
                    var listItemInfo = listItemInfos[i];
                    var parentListItemInfo = listItemInfo;
                    var listItemIsList = listItemInfo.ListItem.IsList();

                    if (multiLevelNumeration)
                        parentListItemInfo.ListItem.SetNumberToListItem(listItemInfo.Number);

                    var insertAfterNode = GetFirstInsertAfterItemList(listItemInfos, i);

                    foreach (var childListItemInfo in listItemInfo.ChildItemInfos)
                    {
                        if (childListItemInfo.ListItem.IsList())
                        {
                            childListItemInfo.ListItem.Remove();
                            continue;
                        }

                        if (multiLevelNumeration)
                            childListItemInfo.ListItem.SetNumberToListItem(childListItemInfo.Number);

                        insertAfterNode = insertAfterNode != null
                            ? parentList.InsertAfter(childListItemInfo.ListItem, insertAfterNode)
                            : parentList.AppendChild(childListItemInfo.ListItem);
                    }

                    if (listItemIsList)
                        parentListItemInfo.ListItem.Remove();
                }
            }

            return listWasChanged;
        }

        /// <summary>
        /// Поиск первого элемента списка (не являющегося списком), после которого будут вставлены дочерние элементы
        /// </summary>
        /// <param name="listItems"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private static HtmlNode GetFirstInsertAfterItemList(IList<ListItemInfo> listItems, int index)
        {
            if (!listItems[index].ListItem.IsList())
                return listItems[index].ListItem;

            for (var i = index - 1; i >= 0; i--)
            {
                if (!listItems[i].ListItem.IsList())
                    return listItems[i].ListItem;
            }

            return null;
        }

        private static bool IsList(this HtmlNode node) =>
            new[] { "ul", "ol" }.Contains(node.Name, StringComparer.OrdinalIgnoreCase);

        private static void SetNumberToListItem(this HtmlNode listItem, string number)
        {
            if (!string.IsNullOrEmpty(number))
                listItem.InnerHtml = number + ".&nbsp;" + listItem.InnerHtml;
        }

        /// <summary>
        /// Получение номера родительского элемента списка, как составной части номера дочернего элемента списка
        /// </summary>
        /// <param name="listItemNumber"></param>
        /// <returns></returns>
        private static string GetParentNumberFromItemListNumber(string listItemNumber)
        {
            if (string.IsNullOrEmpty(listItemNumber))
                return null;

            listItemNumber = listItemNumber.Trim();

            // Откидываем последнюю часть номера
            var lastDotIndex = listItemNumber.LastIndexOf('.');

            return lastDotIndex < 0 ? null : listItemNumber[..lastDotIndex];
        }

        /// <summary>
        /// Получение информации о номере элемента списка
        /// </summary>
        /// <param name="listItem"></param>
        /// <param name="listItemsDict"></param>
        /// <returns></returns>
        private static ListItemInfo GetListItemNumberInfo(HtmlNode listItem,
            IDictionary<HtmlNode, ListItemInfo> listItemsDict)
        {
            if (!listItemsDict.TryGetValue(listItem, out var listItemInfo))
            {
                var listItemText = listItem.InnerText.TrimStart();

                // Выделяем номер по маске с начала строки: <число><точка><любая вариация пробельного символа или табуляции>
                var match = Regex.Match(listItemText,
                    @"(?i)^(\d+(\.\d+)*)\.(&nbsp;|&#xa0;|&#160;|\s|&Tab;|&#9;|&ensp;|&#8194;|&emsp;|&#8195;)");

                var fullNumber = match.Success ? match.Groups[0].Value : null;
                var number = !string.IsNullOrEmpty(fullNumber) ? GetParentNumberFromItemListNumber(fullNumber) : null;
                var parentNumber = !string.IsNullOrEmpty(number) ? GetParentNumberFromItemListNumber(number) : null;

                listItemInfo = new ListItemInfo
                {
                    ListItem = listItem,
                    FullNumber = fullNumber,
                    Number = number,
                    ParentNumber = parentNumber
                };

                listItemsDict.Add(listItem, listItemInfo);
            }

            return listItemInfo;
        }

        /// <summary>
        /// Информация об элементе списка
        /// </summary>
        private class ListItemInfo
        {
            public HtmlNode ListItem { get; set; }
            public string FullNumber { get; init; }
            public string Number { get; init; }


            public int? GetIntNumber()
            {
                return !string.IsNullOrEmpty(Number) && int.TryParse(Number, out var intVal) ? intVal : null;
            }

            public string ParentNumber { get; init; }

            //public HtmlNode ParentList { get; set; }
            public bool WasNumberRemovedFromName { get; set; }

            public List<ListItemInfo> ChildItemInfos { get; } = new List<ListItemInfo>();
        }

        /// <summary>
        /// Сформировать номер для элемента списка
        /// </summary>
        /// <param name="parentNumberPath"></param>
        /// <param name="itemNumber"></param>
        /// <returns></returns>
        private static string GetListItemNumber(string parentNumberPath, int itemNumber) =>
            (!string.IsNullOrEmpty(parentNumberPath) ? parentNumberPath + "." : string.Empty) +
            $"{itemNumber}";

        /// <summary>
        /// Получить стартовый номер для элементов списка
        /// </summary>
        /// <param name="listNode"></param>
        /// <returns></returns>
        private static int GetListItemStartNumber(this HtmlNode listNode)
        {
            return int.TryParse(listNode.GetAttributeValue("start", null) ?? "1", out var itemListStartNumbers)
                ? itemListStartNumbers
                : 1;
        }

        /// <summary>
        /// Получить вышестоящий список для заданного узла (тега)
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private static HtmlNode GetUpperList(this HtmlNode node) =>
            node.FindFirstAncestor(new[] { "ul", "ol" }, false);

        /// <summary>
        /// Получить перечень списков верхнего уровня для заданного узла (тега)
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private static IEnumerable<HtmlNode> GetTopLevelLists(this HtmlNode node)
        {
            var listTags = new[] { "ul", "ol" };

            return node.Descendants()
                .Where(n => listTags.Contains(n.Name, StringComparer.OrdinalIgnoreCase) &&
                            !n.Ancestors().TakeWhile(curNode => curNode != node).Any(a => listTags.Contains(a.Name)));
            //node.Descendants("ol").Concat(node.Descendants("ul")).ToList();
        }

        /// <summary>
        /// Получить перечень элементов верхнего уровня для заданного списка.
        /// Элементом может быть как непосредственно элемент списка, так и списки лежащие внутри списка,
        /// но не внутри другого элемента или списка
        /// </summary>
        /// <param name="listNode"></param>
        /// <returns></returns>
        private static IEnumerable<HtmlNode> GetTopLevelItems(this HtmlNode listNode)
        {
            var parentTagsForInnerListItems = new[] { "li", "ol", "ul" };
            var childTagsForInnerListItems = new[] { "li", "ol", "ul" };

            return listNode.Descendants()
                .Where(n => childTagsForInnerListItems.Contains(n.Name, StringComparer.OrdinalIgnoreCase) &&
                            !n.Ancestors().TakeWhile(curNode => curNode != listNode).Any(a =>
                                parentTagsForInnerListItems.Contains(a.Name, StringComparer.OrdinalIgnoreCase)));
            //a.Name.Equals("li", StringComparison.CurrentCultureIgnoreCase) || ));
            //node.Descendants("ol").Concat(node.Descendants("ul")).ToList();
        }

        #endregion

        /// <summary>
        /// Получить для заданного узла самого первого предка из перечня заданных тегов, с учетом того,
        /// что сам заданный узел может быть вернут в качестве результата, если его имя будет входить в перечень заданных тегов
        /// </summary>
        /// <param name="node"></param>
        /// <param name="tagNames"></param>
        /// <param name="checkSelf"></param>
        /// <returns></returns>
        private static HtmlNode FindFirstAncestor(this HtmlNode node, string[] tagNames, bool checkSelf)
        {
            if (node == null)
                return null;

            if (checkSelf && tagNames.Any(s => s.Equals(node.Name, StringComparison.OrdinalIgnoreCase)))
                return node;

            return _FindFirstAncestor(node.ParentNode, tagNames);
        }

        /// <summary>
        /// Получить для заданного узла самого первого предка из перечня заданных тегов
        /// </summary>
        /// <param name="node"></param>
        /// <param name="tagNames"></param>
        /// <returns></returns>
        private static HtmlNode _FindFirstAncestor(HtmlNode node, string[] tagNames)
        {
            var currentNode = node;

            while (currentNode != null)
            {
                if (tagNames.Any(s => s.Equals(currentNode.Name, StringComparison.OrdinalIgnoreCase)))
                {
                    return currentNode;
                }

                currentNode = currentNode.ParentNode;
            }

            return null;
        }

        public static string ToHex(this Color c)
        {
            return "#" + c.R.ToString("X2") + c.G.ToString("X2") + c.B.ToString("X2");
        }

        public static string ConvertRgbStringToHexString(string rgbValue)
        {
            if (rgbValue.StartsWith("#"))
                return rgbValue;

            var match = Regex.Match(rgbValue, @"\((\d+),\s*(\d+),\s*(\d+)\)");

            if (match.Success)
            {
                var red = int.Parse(match.Groups[1].Value);
                var green = int.Parse(match.Groups[2].Value);
                var blue = int.Parse(match.Groups[3].Value);

                // Формируем hex представление
                var hexColor = $"#{red:X2}{green:X2}{blue:X2}";
                return hexColor;
            }

            return rgbValue;
        }

        /// <summary>
        /// Получение объекта html из строки
        /// </summary>
        /// <param name="htmlData"></param>
        /// <returns></returns>
        public static HtmlNode GetHtmlNodeFromText(string htmlData)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(htmlData);

            return doc.DocumentNode;
        }

        public static string WriteSpaceForEmptyCell(string cellValue)
        {
            var node = GetHtmlNodeFromText(cellValue);

            var nodeInnerText = node.InnerText;

            if (string.IsNullOrEmpty(nodeInnerText))
            {
                //if (!string.IsNullOrEmpty(node.InnerHtml))
                //{
                //}

                node.InnerHtml += "&nbsp;";
            }

            return node.OuterHtml;
        }

        /// <summary>
        /// Удалить пустые теги вида <a name="" />
        /// </summary>
        /// <returns></returns>
        private static string RemoveEmptyATags(string htmlData) =>
            Regex.Replace(htmlData, @"<a\s+name=""[^""]*""></a>", string.Empty);

        /// <summary>
        /// Удалить символы отступов (форматирование)
        /// </summary>
        /// <param name="htmlData"></param>
        /// <returns></returns>
        private static string RemoveFormatting(string htmlData) => Regex.Replace(htmlData, @"\r\n\t+", string.Empty);

        private static string RemoveExcessPhrases(string htmlData) =>
            htmlData.Replace("Export to CSV", string.Empty).Replace("Развернуть исходный код", string.Empty)
                .Replace("Свернуть исходный код", string.Empty).Replace("Развернуть...", string.Empty);

        /// <summary>
        /// Получить перечень наименований классов из атрибута class
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private static IEnumerable<string> GetClassAttributeValues(HtmlNode node)
        {
            var classValue = node.GetAttributeValue("class", null);

            return !string.IsNullOrEmpty(classValue)
                ? classValue.Split(' ')
                : Array.Empty<string>(); // new string[] { };
        }

        /// <summary>
        /// Обработать класс стиля
        /// Функция  для тега <span class=""/> - заменить значения класса стиля на соответствующие теги
        /// </summary>
        /// <returns></returns>
        private static string ProcessSpanTagClassAttribute(string html, bool processTextColor,
            StyleClassesRegistry styleClassesRegistry)
        {
            var docNode = GetHtmlNodeFromText(html);

            foreach (var spanTag in docNode.Descendants("span").ToList())
            {
                ProcessSpanTagClassAttribute(spanTag, processTextColor, styleClassesRegistry);
            }

            return docNode.OuterHtml;
        }

        private static void ProcessSpanTagClassAttribute(HtmlNode spanTag, bool processTextColor,
            StyleClassesRegistry styleClassesRegistry)
        {
            if (styleClassesRegistry == null)
                return;

            foreach (var className in GetClassAttributeValues(spanTag))
            {
                if (styleClassesRegistry.Items.TryGetValue(className, out var styleClass))
                {
                    var innerHtml = spanTag.InnerHtml;

                    // Color
                    if (processTextColor && styleClass.ParametersDict.TryGetValue("color", out var colorValue))
                        innerHtml = WrapTag(innerHtml, "font", "color", colorValue.Value);

                    // Bold
                    if (styleClass.ParametersDict.ContainsKey("font-weight") && styleClass
                            .ParametersDict["font-weight"].Value
                            .Equals("bold", StringComparison.OrdinalIgnoreCase))
                        innerHtml = WrapTag(innerHtml, "b");

                    // Italic
                    if (styleClass.ParametersDict.ContainsKey("font-style") && styleClass
                            .ParametersDict["font-style"].Value
                            .Equals("italic", StringComparison.OrdinalIgnoreCase))
                        innerHtml = WrapTag(innerHtml, "em");

                    // Underline
                    if (styleClass.ParametersDict.ContainsKey("text-decoration") && styleClass
                            .ParametersDict["text-decoration"].Value
                            .Equals("underline", StringComparison.OrdinalIgnoreCase))
                        innerHtml = WrapTag(innerHtml, "u");


                    spanTag.InnerHtml = innerHtml;
                }
            }
        }

        /// <summary>
        /// Выполнить обрамление содержимого HTML тега с атрибутами
        /// </summary>
        /// <param name="innerHtml"></param>
        /// <param name="tagName"></param>
        /// <param name="attributeName"></param>
        /// <param name="attributeValue"></param>
        /// <returns></returns>
        public static string WrapTag(string innerHtml, string tagName, string attributeName = null,
            string attributeValue = null)
        {
            var tag = new HtmlNode(HtmlNodeType.Element, new HtmlDocument(), 0)
            {
                Name = tagName
            };

            if (!string.IsNullOrEmpty(attributeName) && !string.IsNullOrEmpty(attributeValue))
                tag.Attributes.Add(attributeName, attributeValue);

            tag.InnerHtml = innerHtml;
            return tag.OuterHtml;
        }

        /// <summary>
        /// Обработка тега <span style="..."/>, убираем атрибут style, заменяя его на:
        ///  - "color: rgb(0,128,0)" на <font color="rgb(0,128,0)"/>
        ///  - "font-weight: bolder" на <b/>
        /// </summary>
        /// <param name="html"></param>
        /// <param name="processTextColor"></param>
        /// <returns></returns>
        public static string ProcessSpanTagStyleAttribute(string html, bool processTextColor)
        {
            var docNode = HtmlUtils.GetHtmlNodeFromText(html);

            foreach (var spanTag in docNode.Descendants("span").ToList())
            {
                ProcessSpanTagStyleAttribute(spanTag, processTextColor);
            }

            return docNode.OuterHtml;
        }

        private static void ProcessSpanTagStyleAttribute(HtmlNode spanTag, bool processTextColor)
        {
            var styleValue = spanTag.GetAttributeValue("style", null);

            if (styleValue != null)
            {
                var innerHtml = spanTag.InnerHtml;

                // Color
                if (processTextColor)
                {
                    var colorValueMatch = Regex.Match(styleValue,
                        @"(?<![-\w])color\s*:\s*(#(?:[0-9a-fA-F]{3}){1,2}|rgb\(\s*\d+\s*,\s*\d+\s*,\s*\d+\s*\))",
                        RegexOptions.IgnoreCase);

                    if (colorValueMatch.Success)
                    {
                        var colorValue = colorValueMatch.Groups[1].Value;
                        colorValue = HtmlUtils.ConvertRgbStringToHexString(colorValue);
                        innerHtml = HtmlUtils.WrapTag(innerHtml, "font", "color", colorValue);
                    }
                }

                var styleValueWithoutSpaces = styleValue.Replace(" ", string.Empty);

                // Bold
                if (styleValueWithoutSpaces.Contains("font-weight:bolder", StringComparison.OrdinalIgnoreCase) ||
                    styleValueWithoutSpaces.Contains("font-weight:bold"))
                {
                    innerHtml = HtmlUtils.WrapTag(innerHtml, "b");
                }

                // Italic
                if (styleValueWithoutSpaces.Contains("font-style:italic", StringComparison.OrdinalIgnoreCase))
                {
                    innerHtml = HtmlUtils.WrapTag(innerHtml, "em");
                }

                // UnderLine
                if (styleValueWithoutSpaces.Contains("text-decoration:underline", StringComparison.OrdinalIgnoreCase))
                {
                    innerHtml = HtmlUtils.WrapTag(innerHtml, "u");
                }

                spanTag.InnerHtml = innerHtml;
            }
        }

        private static string RemoveLeadingAndTrailingTokens(string htmlData, string tokenToRemove) =>
            Regex.Replace(htmlData, $@"^\s*({tokenToRemove})+|\s*({tokenToRemove})+$", string.Empty);

        /// <summary>
        /// Заменить все вхождения тега на другой тег
        /// </summary>
        /// <returns></returns>
        private static string ReplaceTag(string html, string sourceTag, string targetTag) => html
            .Replace($"<{sourceTag}>", $"<{targetTag}>").Replace($"</{sourceTag}>", $"</{targetTag}>");

        /// <summary>
        /// Заменить один тег на другой с сохранением атрибутов
        /// </summary>
        /// <param name="html"></param>
        /// <param name="sourceTag"></param>
        /// <param name="targetTag"></param>
        /// <returns></returns>
        private static string ReplaceTagHavingAttributes(string html, string sourceTag, string targetTag) =>
            ReplaceTagHavingAttributes(HtmlUtils.GetHtmlNodeFromText(html), sourceTag, targetTag).OuterHtml;

        private static HtmlNode ReplaceTagHavingAttributes(HtmlNode node, string sourceTag, string targetTag)
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

        private static string RemoveFontColorTags(string htmlData)
        {
            const string pattern = @"<font[^>]*color=[""']([^""']*)[""'][^>]*>(.*?)</font>";
            var result = Regex.Replace(htmlData, pattern, "$2");

            return result;
        }

        /// <summary>
        /// Найти заданные теги с заданным стилем и произвести над ними заданное действие
        /// </summary>
        /// <param name="html"></param>
        /// <param name="tag"></param>
        /// <param name="className"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        private static string ProcessTagsWithClass(string html, string tag, string className,
            TagProcessActionType action)
        {
            var docNode = HtmlUtils.GetHtmlNodeFromText(html);

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

        private static string ReplaceTagsInsideTableCellWithCodeClass(string html, string sourceTag, string targetTag)
        {
            var docNode = HtmlUtils.GetHtmlNodeFromText(html);

            var tdNodes = docNode.Descendants("td")
                .Where(td =>
                    td.Attributes["class"] != null &&
                    td.Attributes["class"].Value.Equals("code", StringComparison.OrdinalIgnoreCase));

            foreach (var node in tdNodes)
            {
                ReplaceTagHavingAttributes(node, sourceTag, targetTag);
            }

            return docNode.OuterHtml;
        }

        /// <summary>
        /// Удалить лишние переносы
        /// </summary>
        /// <param name="cellValue"></param>
        /// <returns></returns>
        private static string RemoveNewLines(string cellValue) => cellValue.Replace("\r\n", string.Empty)
            .Replace("\n", string.Empty).Replace("\r", string.Empty);


        private static string RemoveExcessAttributes(string html, string[] excessAttributes)
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
                                  NoProcessStyleClasses.Contains(classNames[0])))
                                element.Attributes.Remove("class");
                        }
                        else element.Attributes.Remove(attribute);
                    }
                }
            }

            return doc.DocumentNode.OuterHtml;
        }

        private static string RemovePTagsAtStartAndEnd(string htmlData)
        {
            var docNode = HtmlUtils.GetHtmlNodeFromText(htmlData);

            if (docNode.ChildNodes.Count == 1 &&
                docNode.FirstChild.Name.Equals("p",
                    StringComparison
                        .OrdinalIgnoreCase)) // Удаляем тег p в любом случае, т.к. выравнивание по абзацу отстутствует в Сфера.Документы
            {
                //if (doc.DocumentNode.FirstChild.Attributes.Count == 0 || cellIsHeader) // Не имеет смысла, т.к. выравнивание текста все равно не применяется в Сфера.Документы
                return docNode.FirstChild.InnerHtml;
            }

            return htmlData;

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

        /// <summary>
        /// Удаление тегов, не имеющих значений
        /// </summary>
        /// <param name="cellValue"></param>
        /// <returns></returns>
        private static string RemoveEmptyValueTags(string cellValue)
        {
            if (cellValue.Contains("<img", StringComparison.OrdinalIgnoreCase)) // Оставляем изображения как есть
                return cellValue;

            var cellNodeInnerText = HtmlUtils.GetHtmlNodeFromText(cellValue).InnerText.Trim();
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

        //private string ProcessSpanTagClassAttribute(string html, bool processTextColor)
        //{
        //    return Utils.ProcessSpanTagClassAttribute(html, processTextColor, _styleClassesRegistry);
        //    //var docNode = Utils.GetHtmlNodeFromText(html);

        //    //foreach (var spanTag in docNode.Descendants("span").ToList())
        //    //{
        //    //    ProcessSpanTagClassAttribute(spanTag, processTextColor);
        //    //}

        //    //return docNode.OuterHtml;
        //}

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

        /// Обработка тега <span style="..."/>, для значений style добавляем дополнительные теги:
        ///  - "color: rgb(0,128,0)" на <font color="rgb(0,128,0)"/>
        ///  - "font-weight: bolder" на <b/>
        //private string ProcessSpanTagStyleAttribute(string html, bool processTextColor)
        //{
        //return Utils.ProcessSpanTagStyleAttribute(html, processTextColor);
        //var docNode = Utils.GetHtmlNodeFromText(html);

        //foreach (var spanTag in docNode.Descendants("span").ToList())
        //{
        //    ProcessSpanTagStyleAttribute(spanTag, processTextColor);
        //}

        //return docNode.OuterHtml;
        //}

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


        //private string ProcessPTagClassStyle(string cellValue)
        //{
        //    var doc = new HtmlDocument();
        //    doc.LoadHtml(cellValue);

        //    foreach (var tag in doc.DocumentNode.Descendants("p").ToList())
        //    {
        //        foreach (var className in HtmlUtils.GetClassAttributeValues(tag))
        //        {
        //            if (_styleClassesRegistry.Items.TryGetValue(className, out var styleClass))
        //            {
        //                //var innerHtml = tag.InnerHtml;

        //                if (styleClass.ParametersDict.TryGetValue("text-align", out var alignValue)) // Выравнивание не имеет смысла, т.к. неможет быть применено в Сфера.Документы
        //                    tag.SetAttributeValue("align", alignValue.Value);
        //            }
        //        }
        //    }

        //    return doc.DocumentNode.OuterHtml;
        //}


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
        //private void ReplaceClassByStyle(HtmlNode node)
        //{
        //    var elementsWithClass = node.Descendants().Where(e => e.Attributes.Contains("class"));

        //    foreach (var element in elementsWithClass)
        //    {
        //        var classValue = element.GetAttributeValue("class", string.Empty);

        //        // Разбираем значения class на отдельные классы
        //        var classNames = classValue.Split(' ');

        //        if (classNames.Length == 1 &&
        //            _noProcessStyleClasses.Contains(classNames[0]))
        //            continue;

        //        // Получаем значение атрибута style на основе классов
        //        var styleValue = GetStyleValueOnClassValues(classNames);

        //        element.SetAttributeValue("style", styleValue);
        //        element.Attributes.Remove("class");
        //    }

        //}

        /// <summary>
        /// Формирование значения атрибута style на основе значений классов
        /// </summary>
        //private string GetStyleValueOnClassValues(string[] classNames)
        //{
        //    //PrepareStylesClassesDict();
        //    var result = string.Empty;

        //    foreach (var className in classNames)
        //    {
        //        if (_styleClassesRegistry.Items.TryGetValue(className, out var styleClass))
        //        {
        //            result = styleClass.Parameters.Aggregate(result,
        //                (current, styleClassParameter) => current + styleClassParameter);
        //        }
        //    }

        //    return result;
        //}


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
        public static bool IsCellHeader(HtmlNode cell, StyleClassesRegistry styleClassesRegistry)
        {
            if (cell.Name.Equals("th", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            // Берем наименования классов у ячейки
            var classAttributeValue = cell.GetAttributeValue("class", null);
            if (!string.IsNullOrEmpty(classAttributeValue))
            {
                if (classAttributeValue.Split(' ').Any(className =>
                        HasBackgroundColorInStyleClass(className, styleClassesRegistry) ||
                        className.Equals("highlight-grey", StringComparison.OrdinalIgnoreCase)))
                {
                    return true;
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
                if (IsColorRefersToHeaderColors(
                        bgСolorValue)) //bgcolorValue.Equals(Utils.TableHeaderBackgroundColorHexValue, StringComparison.OrdinalIgnoreCase))
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
        public static bool IsColorRefersToHeaderColors(string colorHexValue)
        {
            return HtmlUtils.TableHeaderColors.Any(color =>
                color.HexValue.Equals(colorHexValue, StringComparison.OrdinalIgnoreCase));
        }

        public static bool HasBackgroundColorInStyleClass(string className, StyleClassesRegistry styleClassesRegistry)
        {
            //PrepareStylesClassesDict();

            if (styleClassesRegistry.Items.TryGetValue(className, out var styleClass))
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
        public static int GetXWithColspan(int currentY, int currentX, IReadOnlyDictionary<int, int> colspanMap)
        {
            var x = currentX;

            while (colspanMap.TryGetValue(x, out var y) && y > currentY)
            {
                x++;
            }

            return x;
        }

        public static bool IsNodeInsideNestedTable(HtmlNode mainTable, HtmlNode node)
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

        /// <summary>
        /// Удаление тегов <b> и </b>
        /// </summary>
        /// <param name="cellValue"></param>
        /// <returns></returns>
        private static string RemoveBoldTags(string cellValue) => Regex.Replace(cellValue, "<[/]?b>", string.Empty);

        /// <summary>
        /// Проверка на то, что ячейка является вспомогательной, скрытой
        /// </summary>
        /// <param name="cell"></param>
        /// <returns></returns>
        public static bool IsCellVisible(HtmlNode cell)
        {
            var styleValue = cell.GetAttributeValue("style", String.Empty);

            if (!string.IsNullOrEmpty(styleValue) &&
                styleValue.Contains("display: none;", StringComparison.OrdinalIgnoreCase))
                return false;

            return true;
        }

        public static bool IsRowVisible(HtmlNode row)
        {
            var attributeValue = row.GetAttributeValue("contenteditable", string.Empty);

            if (!string.IsNullOrEmpty(attributeValue) &&
                attributeValue.Equals("false", StringComparison.OrdinalIgnoreCase))
                return false;

            return true;
        }

        #region Постобработка полученных ячеек - корректировка высоты ячеек для учета возможного бага в html разметке

        /// <summary>
        /// Постобработка полученных ячеек для устранения багов с высотой ячеек в html разметке от DevExpress
        /// </summary>
        /// <param name="cells"></param>
        internal static void PostProcessCells(this IReadOnlyList<Cell> cells)
        {
            foreach (var targetCell in cells.Where(c => c.H > 1)) // Получаем ячейки объединенные по высоте
            {
                if (cells.Any(c =>
                        c.Y == targetCell.Y + targetCell.H - 1 && c.W > 1 && targetCell.IntersectsByWidth(c)))
                    targetCell.H--;
            }
        }

        #endregion

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
}