using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using static TransfromService.Html2JsonTransformer;

namespace TransfromService.JsonData
{
    internal class RootContent
    {
        [JsonProperty("title", NullValueHandling = NullValueHandling.Ignore)]
        public string Title { get; set; }

        [JsonProperty("table")] public Table Table { get; set; }
    }
}