using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using HtmlDocument = HtmlAgilityPack.HtmlDocument;

namespace TransfromUtils
{
    public class TableTransformerOld
    {
        public string Transform(string htmlText)
        {
            string result = null;

            if (string.IsNullOrEmpty(htmlText.Trim()))
                return result;

            // Создание JSON-структуры для хранения данных таблицы
            Root root = new Root
            {
                Content = new RootContent
                {
                    Table = new Table
                    {
                        Cells = new List<Cell>() // Изменено на список Cell
                    }
                }
            };

            var doc = new HtmlDocument();
            doc.LoadHtml(htmlText);

            var rows = doc.DocumentNode.SelectNodes("//tr");

            if (rows == null)
                return result;

            int y = 0;
            Dictionary<int, int> colspanMap = new Dictionary<int, int>();

            foreach (var row in rows)
            {
                var cells = row.SelectNodes(".//td | .//th");
                if (cells == null)
                    return result;

                int x = 0;

                foreach (var cell in cells)
                {
                    var cellText = cell.InnerText.Trim();
                    bool isHeader = cell.Name.Equals("th", StringComparison.OrdinalIgnoreCase);

                    int colspan = 1;
                    int rowspan = 1;

                    var colspanAttribute = cell.Attributes["colspan"];
                    if (colspanAttribute != null && int.TryParse(colspanAttribute.Value, out int colspanValue))
                    {
                        colspan = colspanValue;
                    }

                    var rowspanAttribute = cell.Attributes["rowspan"];
                    if (rowspanAttribute != null && int.TryParse(rowspanAttribute.Value, out int rowspanValue))
                    {
                        rowspan = rowspanValue;
                    }

                    // Вычисляем значение x с учетом объединенных ячеек
                    x = GetXWithColspan(y, x, colspanMap);

                    Cell cellData = new Cell
                    {
                        X = x,
                        Y = y,
                        W = colspan,
                        H = rowspan,
                        IsHeader = isHeader ? true : (bool?)null,
                        Items = new List<Item>
                        {
                            new Item
                            {
                                Uuid = Guid.NewGuid().ToString(),
                                Type = "TEXT",
                                Content = new ItemContent
                                {
                                    Value = $"<p>{cellText}</p>"
                                }
                            }
                        }
                    };

                    // Обновляем карту объединенных ячеек для текущей строки
                    for (int i = x; i < x + colspan; i++)
                    {
                        colspanMap[i] = y + rowspan;
                    }

                    root.Content.Table.Cells.Add(cellData);
                    x += colspan;
                }

                y++;
            }

            result = JsonConvert.SerializeObject(root, Newtonsoft.Json.Formatting.Indented);

            return result;
        }

        // Метод для определения значения x с учетом объединенных ячеек
        private int GetXWithColspan(int y, int currentX, Dictionary<int, int> colspanMap)
        {
            int x = currentX;

            while (colspanMap.ContainsKey(x) && colspanMap[x] > y)
            {
                x++;
            }

            return x;
        }

        private class Root
        {
            [JsonProperty("type")]
            public string Type { get; set; } = "TABLE";

            [JsonProperty("content")]
            public RootContent Content { get; set; }
        }

        private class RootContent
        {
            [JsonProperty("table")]
            public Table Table { get; set; }
        }

        private class Table
        {
            [JsonProperty("cells")]
            public List<Cell> Cells { get; set; }
        }

        private class Cell
        {
            [JsonProperty("x")]
            public int X { get; set; }

            [JsonProperty("y")]
            public int Y { get; set; }

            [JsonProperty("w")]
            public int W { get; set; }

            [JsonProperty("h")]
            public int H { get; set; }

            [JsonProperty("isHeader")]
            public bool? IsHeader { get; set; }

            [JsonProperty("items")]
            public List<Item> Items { get; set; }
        }

        private class Item
        {
            [JsonProperty("uuid")]
            public string Uuid { get; set; }
            [JsonProperty("type")]
            public string Type { get; set; }
            [JsonProperty("content")]
            public ItemContent Content { get; set; }
        }

        private class ItemContent
        {
            [JsonProperty("value")]
            public string Value { get; set; }
        }
    }
}
