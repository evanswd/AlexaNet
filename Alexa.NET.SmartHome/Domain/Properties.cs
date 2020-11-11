using Newtonsoft.Json;

namespace Alexa.NET.SmartHome.Domain
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class Properties
    {
        [JsonProperty("supported")]
        public Supported[] Supported { get; set; }

        [JsonProperty("proactivelyReported")]
        public bool ProactivelyReported { get; set; }

        [JsonProperty("retrievable")]
        public bool Retrievable { get; set; }
    }
}
