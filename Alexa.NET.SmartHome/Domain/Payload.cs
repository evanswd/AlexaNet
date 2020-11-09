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
    }
}
