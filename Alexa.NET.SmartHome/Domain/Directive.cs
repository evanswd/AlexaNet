using Newtonsoft.Json;

namespace Alexa.NET.SmartHome.Domain
{
    public class Directive
    {
        [JsonProperty("header")] 
        public Header Header { get; set; }

        [JsonProperty("payload")]
        public Payload Payload { get; set; }
    }
}
