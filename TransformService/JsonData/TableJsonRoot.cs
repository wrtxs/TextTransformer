using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransformService.JsonData
{
    internal class TableJsonRoot : TypedJsonRootBase<TableRootContent>
    {
        public override ContentType Type => ContentType.Table;

        public static TableJsonRoot GetRootInstanceForTable(TableMetadata.TableMetadata tableMetadata) =>
            new()
            {
                Content = new TableRootContent
                {
                    Title = tableMetadata.Title,
                    Table = new Table
                    {
                        Widths = new List<int>(tableMetadata.ColumnWidths)
                        //Cells = new List<Cell>()
                    }
                }
            };
    }

    internal class TableRootContent : RootContentBase
    {
        [JsonProperty("title", NullValueHandling = NullValueHandling.Ignore)]
        public string Title { get; set; }

        [JsonProperty("table")] public Table Table { get; set; }
    }

    internal class Table
    {
        [JsonProperty("widths")] public List<int> Widths { get; init; } = new();
        [JsonProperty("cells")] public List<Cell> Cells { get; } = new();
    }

    internal class Cell
    {
        [JsonProperty("x")] public int X { get; set; }

        [JsonProperty("y")] public int Y { get; set; }

        [JsonProperty("w")] public int W { get; set; }

        [JsonProperty("h")] public int H { get; set; }
        
        [JsonProperty("isAutonumbered", NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsAutoNumbered { get; set; }
        
        [JsonProperty("isHeader", NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsHeader { get; set; }

        [JsonProperty("items")] public List<Item> Items { get; set; }

        public bool IntersectsByWidth(Cell otherCell) =>
            X < (otherCell.X + otherCell.W) && (X + W) > otherCell.X;
    }

    internal class Item
    {
        [JsonProperty("uuid")] public string Uuid { get; set; }
        [JsonProperty("type")] public string Type { get; set; }
        [JsonProperty("content")] public ItemContent Content { get; set; }
    }

    internal class ItemContent
    {
        [JsonProperty("value")]
        public string Value { get; set; }
    }
}
