using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using static TransfromService.Html2JsonTransformer;

namespace TransfromService.JsonData
{
    internal class Cell
    {
        [JsonProperty("x")]
        public int X { get; set; }

        [JsonProperty("y")]
        public int Y { get; set; }

        [JsonProperty("w")]
        public int W { get; set; }

        [JsonProperty("h")]
        public int H { get; set; }

        [JsonProperty("isHeader", NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsHeader { get; set; }

        [JsonProperty("items")]
        public List<Item> Items { get; set; }
    }
}
