using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace TransformService.JsonData
{
    internal abstract class JsonRootBase
    {
        [JsonConverter(typeof(StringEnumConverter)), JsonProperty("type")]
        public abstract ContentType Type { get; }
    }


    internal abstract class TypedJsonRootBase<T> : JsonRootBase where T : RootContentBase
    {
        [JsonProperty("id")] public int Id { get; } = new Random().Next(10000000, 100000000);
        
        [JsonProperty("content")] public T Content { get; set; }
    }

    internal abstract class RootContentBase
    {
    }

    //[JsonConverter(typeof(StringEnumConverter))]
    internal enum ContentType
    {
        [Description("UNKNOWN"), EnumMember(Value = "UNKNOWN")] Unknown,
        [Description("TABLE"), EnumMember(Value = "TABLE")] Table,
        [Description("TEXT"), EnumMember(Value = "TEXT")] Text
    }
}
