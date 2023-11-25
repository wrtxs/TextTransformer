using Newtonsoft.Json;
using System.Collections.Generic;
using static TransfromService.Html2JsonTransformer;

namespace TransfromService.JsonData
{
    internal class Root
    {
        [JsonProperty("type")] public string Type { get; set; } = "TABLE";

        [JsonProperty("content")] public RootContent Content { get; set; }


        public static Root GetRootInstance(string tableTitle) =>
            new()
            {
                Content = new RootContent
                {
                    Title = tableTitle,
                    Table = new Table
                    {
                        Cells = new List<Cell>()
                    }
                }
            };
    }
}
