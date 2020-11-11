using Newtonsoft.Json;

namespace Alexa.NET.SmartHome.Domain.Payloads
{
    public class SetVolumePayload : Payload
    {
        [JsonProperty("volume")]
        public int Volume { get; set; }
    }
}
