using Newtonsoft.Json;

namespace Alexa.NET.SmartHome.Domain.Payloads
{
    public class SetMutePayload : Payload
    {
        [JsonProperty("mute")]
        public bool Mute { get; set; }
    }
}
