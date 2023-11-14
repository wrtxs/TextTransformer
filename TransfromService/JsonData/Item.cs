using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using static TransfromService.Html2JsonTransformer;

namespace TransfromService.JsonData
{
    internal class Item
    {
        [JsonProperty("uuid")]
        public string Uuid { get; set; }
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("content")]
        public ItemContent Content { get; set; }
    }
}
