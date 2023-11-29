using Newtonsoft.Json;

namespace TransfromService.JsonData
{
    internal class TextJsonRoot : TypedJsonRootBase<TextRootContent>
    {
        public override ContentType Type => ContentType.Text;

        public static TextJsonRoot GetRootInstanceForText() =>
            new()
            {
                Content = new TextRootContent()
            };
    }

    internal class TextRootContent : RootContentBase
    {
        [JsonProperty("value")]
        public string Value { get; set; }
    }
}
