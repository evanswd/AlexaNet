using Newtonsoft.Json;

namespace Alexa.NET.SmartHome.Domain.Payloads;

public class SetPowerLevelPayload : Payload
{
    [JsonProperty("powerLevel")]
    public int PowerLevel { get; set; }
}