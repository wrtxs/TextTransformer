using Newtonsoft.Json;
using System.Collections.Generic;
using static TransfromService.Html2JsonTransformer;

namespace TransfromService.JsonData
{
    internal class Root
    {
        [JsonProperty("type")]
        public string Type { get; set; } = "TABLE";

        [JsonProperty("content")]
        public RootContent Content { get; set; }

        public static Root GetRootInstance()
        {
            return new Root
            {
                Content = new RootContent
                {
                    Table = new Table
                    {
                        Cells = new List<Cell>()
                    }
                }
            };
        }
    }
}
