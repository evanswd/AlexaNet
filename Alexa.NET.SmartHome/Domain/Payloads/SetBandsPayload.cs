using Alexa.NET.SmartHome.Domain.Constants;
using Newtonsoft.Json;

namespace Alexa.NET.SmartHome.Domain.Payloads;

public class SetBandsPayload : Payload
{
    [JsonProperty("bands")]
    public NameValuePair<EqualizerBands, int>[] Bands { get; set; }
}