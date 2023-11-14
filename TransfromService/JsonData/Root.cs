using Newtonsoft.Json;
using static TransfromService.Html2JsonTransformer;

namespace TransfromService.JsonData
{
    internal class Root
    {
        [JsonProperty("type")]
        public string Type { get; set; } = "TABLE";

        [JsonProperty("content")]
        public RootContent Content { get; set; }
    }
}
