using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace TransformService.JsonData
{
    internal class JsonUtils
    {
        public static JsonRootBase DeserializeObject(string jsonData)
        {
            var result = JsonConvert.DeserializeObject<List<JsonRootBase>>(jsonData, new JsonRootConverter());
            var jsonObject = result.FirstOrDefault(item => item is { Type: ContentType.Table }) ??
                             result.FirstOrDefault(item => item != null);

            return jsonObject;
        }

        public static string SerializeObject(JsonRootBase jsonObject, Formatting formatting)
        {
            return JsonConvert.SerializeObject(new List<JsonRootBase> { jsonObject }, formatting, new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
        }
    }
}