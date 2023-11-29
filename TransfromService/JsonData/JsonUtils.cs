using Newtonsoft.Json;

namespace TransfromService.JsonData
{
    internal class JsonUtils
    {
        public static JsonRootBase DeserializeObject(string jsonData)
        {
            return JsonConvert.DeserializeObject<JsonRootBase>(jsonData, new JsonRootConverter());
        }

        public static string SerializeObject(JsonRootBase jsonObject, Formatting formatting)
        {
            return JsonConvert.SerializeObject(jsonObject, formatting, new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
        }
    }
}