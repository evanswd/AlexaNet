using Newtonsoft.Json;

namespace Alexa.NET.SmartHome.Domain.Payloads
{
    public class SetModePayload : Payload
    {
        [JsonProperty("mode")]
        public EqualizerMode Mode { get; set; }
    }
}
