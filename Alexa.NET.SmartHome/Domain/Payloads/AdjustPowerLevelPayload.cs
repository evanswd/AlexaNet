using Newtonsoft.Json;

namespace Alexa.NET.SmartHome.Domain.Payloads;

public class AdjustPowerLevelPayload : Payload
{
    [JsonProperty("powerLevelDelta")]
    public int PowerLevelDelta { get; set; }
}