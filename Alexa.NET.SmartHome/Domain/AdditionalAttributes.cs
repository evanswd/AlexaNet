using Newtonsoft.Json;

namespace Alexa.NET.SmartHome.Domain;

[JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
public class AdditionalAttributes
{
    [JsonProperty("manufacturer")]
    public string Manufacturer { get; set; }

    [JsonProperty("model")]
    public string Model { get; set; }

    [JsonProperty("serialNumber")]
    public string SerialNumber { get; set; }

    [JsonProperty("firmwareVersion")]
    public string FirmwareVersion { get; set; }

    [JsonProperty("softwareVersion")]
    public string SoftwareVersion { get; set; }

    [JsonProperty("customIdentifier")]
    public string CustomIdentifier { get; set; }
}