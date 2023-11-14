using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using static TransfromService.Html2JsonTransformer;

namespace TransfromService.JsonData
{
    internal class Table
    {
        [JsonProperty("cells")]
        public List<Cell> Cells { get; set; }
    }
}
