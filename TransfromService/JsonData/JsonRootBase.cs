﻿using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace TransfromService.JsonData
{
    internal abstract class JsonRootBase
    {
        [JsonConverter(typeof(StringEnumConverter)), JsonProperty("type")]
        public abstract ContentType Type { get; }
    }


    internal abstract class TypedJsonRootBase<T> : JsonRootBase where T : RootContentBase
    {
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
