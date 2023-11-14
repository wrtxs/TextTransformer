using System;
using System.Collections.Generic;
using System.Drawing;
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
    }
}