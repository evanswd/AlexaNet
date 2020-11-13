using Newtonsoft.Json;

namespace Alexa.NET.SmartHome.Domain
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class NameValuePair<TName,TValue>
    {
        [JsonProperty("name")]
        public TName Name { get; set; }

        [JsonProperty("value")]
        public TValue Value { get; set; }
    }
}
