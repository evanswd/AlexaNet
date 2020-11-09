using Newtonsoft.Json;

namespace Alexa.NET.SmartHome.Domain
{
    public class Header
    {   
        [JsonProperty("namespace")]
        public string Namespace { get; set; }

        [JsonProperty("name")] 
        public string Name { get; set; }

        [JsonProperty("messageId")]
        public string MessageID { get; set; }

        [JsonProperty("correlationToken")] 
        public string CorrelationToken { get; set; }

        [JsonProperty("payloadVersion")]
        public string PayloadVersion { get; set; }
    }
}
