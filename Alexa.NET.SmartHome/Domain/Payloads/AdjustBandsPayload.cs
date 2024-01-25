using Alexa.NET.SmartHome.Domain.Constants;
using Newtonsoft.Json;

namespace Alexa.NET.SmartHome.Domain.Payloads;

public class AdjustBandsPayload : Payload
{
    [JsonProperty("bands")]
    public AdjustBandsPayloadItem[] Bands { get; set; }
}

public class AdjustBandsPayloadItem
{
    [JsonProperty("name")]
    public EqualizerBands Name { get; set; }

    [JsonProperty("levelDelta")]
    public int LevelDelta { get; set; }

    [JsonProperty("levelDirection")] 
    public LevelDirection LevelDirection { get; set; }
}