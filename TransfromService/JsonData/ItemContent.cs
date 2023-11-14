using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace TransfromService.JsonData
{
    internal class ItemContent
    {
        [JsonProperty("value")]
        public string Value { get; set; }
    }
}