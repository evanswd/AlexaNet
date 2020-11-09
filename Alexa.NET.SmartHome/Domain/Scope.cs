using Newtonsoft.Json;

namespace Alexa.NET.SmartHome.Domain
{
    public class Scope
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("token")]
        public string Token { get; set; }
    }
}
