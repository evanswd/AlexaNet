using Newtonsoft.Json;

namespace Alexa.NET.SmartHome.Domain
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class Payload
    {   
        [JsonProperty("scope")]
        public Scope Scope { get; set; }

        [JsonProperty("endpoints")]
        public Endpoint[] Endpoints { get; set; }

        [JsonProperty("mute")]
        public bool? Mute { get; set; }

        [JsonProperty("volume")]
        public int? Volume { get; set; }

        [JsonProperty("volumeDefault")]
        public bool? VolumeDefault { get; set; }
    }
}
