using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;

namespace TransfromService.JsonData
{
    internal class JsonRootConverter : JsonConverter<JsonRootBase>
    {
        public override void WriteJson(JsonWriter writer, JsonRootBase value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
            //switch (value)
            //{
            //    case TableJsonRoot tableRoot:
            //        serializer.Serialize(writer, tableRoot);
            //        break;
            //    case TextJsonRoot textRo9ot:
            //        serializer.Serialize(writer, textRoot);
            //        break;
            //    default:
            //        throw new InvalidOperationException("Invalid JSON content");
            //}
        }

        public override JsonRootBase ReadJson(JsonReader reader, Type objectType, JsonRootBase existingValue,
            bool hasExistingValue,
            JsonSerializer serializer)
        {
            var obj = JObject.Load(reader);

            if (Enum.TryParse(typeof(ContentType), obj["type"]!.Value<string>(), true, out var enumValue))
            {
                var objType = (ContentType)enumValue!;

                switch (objType)
                {
                    case ContentType.Table:
                        return obj.ToObject<TableJsonRoot>();
                    case ContentType.Text:
                        return obj.ToObject<TextJsonRoot>();
                }
            }

            throw new InvalidOperationException("Invalid JSON content");
        }
    }
}
