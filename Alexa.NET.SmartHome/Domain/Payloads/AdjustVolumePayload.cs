using Newtonsoft.Json;

namespace Alexa.NET.SmartHome.Domain.Payloads;

public class AdjustVolumePayload : SetVolumePayload
{
    [JsonProperty("volumeDefault")]
    public bool VolumeDefault { get; set; }
}