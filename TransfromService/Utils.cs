using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;

namespace TransfromService
{
    public static class Utils
    {
        //public static Color TableHeaderBackgroundColor { get; }
        //public static string TableHeaderBackgroundColorHexValue { get; }

        //public static int TableHeaderBackgroundColorArgbValue { get; }

        public static CellBackgroundColor CommonTableHeaderColor { get; }

        public static IReadOnlyList<CellBackgroundColor> TableHeaderColors { get; }

        static Utils()
        {
            CommonTableHeaderColor = new CellBackgroundColor(216, 216, 216);

            TableHeaderColors = new List<CellBackgroundColor>
            {
                CommonTableHeaderColor,
                new CellBackgroundColor(244, 245, 247), // Цвет ячейки-заголовка Confluence 
                new CellBackgroundColor(242, 242, 242),
                new CellBackgroundColor(191, 191, 191),
                new CellBackgroundColor(165, 165, 165),
                new CellBackgroundColor(127, 127, 127)
            };


            //TableHeaderBackgroundColor = Color.FromArgb(216, 216, 216);
            //TableHeaderBackgroundColorHexValue = TableHeaderBackgroundColor.ToHex();
            //TableHeaderBackgroundColorArgbValue = TableHeaderBackgroundColor.ToArgb();
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
        /// <param name="htmlText"></param>
        /// <returns></returns>
        public static HtmlNode GetHtmlNodeFromText(string htmlText)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(htmlText);

            return doc.DocumentNode;
        }

        /// <summary>
        /// Получить перечень наименований классов из атрибута class
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public static IEnumerable<string> GetClassAttributeValues(HtmlNode node)
        {
            var classValue = node.GetAttributeValue("class", null);

            return !string.IsNullOrEmpty(classValue)
                ? classValue.Split(' ')
                : Array.Empty<string>(); // new string[] { };
        }

        /// <summary>
        /// Обработать класс стиля
        /// Функция  для тега <span class=""> - заменить значения класса стиля на соответствующие теги
        /// </summary>
        /// <returns></returns>
        public static string ProcessSpanTagClassAttribute(string html, bool processTextColor,
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
        /// Обработка тега <span style="...">, убираем атрибут style, заменяя его на:
        ///  - "color: rgb(0,128,0)" на <font color="rgb(0,128,0)"/>
        ///  - "font-weight: bolder" на <b/>
        /// </summary>
        /// <param name="html"></param>
        /// <param name="processTextColor"></param>
        /// <returns></returns>
        public static string ProcessSpanTagStyleAttribute(string html, bool processTextColor)
        {
            var docNode = Utils.GetHtmlNodeFromText(html);

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
                        colorValue = Utils.ConvertRgbStringToHexString(colorValue);
                        innerHtml = Utils.WrapTag(innerHtml, "font", "color", colorValue);
                    }
                }

                var styleValueWithoutSpaces = styleValue.Replace(" ", string.Empty);

                // Bold
                if (styleValueWithoutSpaces.Contains("font-weight:bolder", StringComparison.OrdinalIgnoreCase) ||
                    styleValueWithoutSpaces.Contains("font-weight:bold"))
                {
                    innerHtml = Utils.WrapTag(innerHtml, "b");
                }

                // Italic
                if (styleValueWithoutSpaces.Contains("font-style:italic", StringComparison.OrdinalIgnoreCase))
                {
                    innerHtml = Utils.WrapTag(innerHtml, "em");
                }

                // UnderLine
                if (styleValueWithoutSpaces.Contains("text-decoration:underline", StringComparison.OrdinalIgnoreCase))
                {
                    innerHtml = Utils.WrapTag(innerHtml, "u");
                }

                spanTag.InnerHtml = innerHtml;
            }
        }
    }
}