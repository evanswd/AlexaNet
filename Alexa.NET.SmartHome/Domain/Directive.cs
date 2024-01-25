using Alexa.NET.SmartHome.Domain.Payloads;
using Newtonsoft.Json;

namespace Alexa.NET.SmartHome.Domain;

[JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
public class Directive<T> where T : Payload
{
    [JsonProperty("header")]
    public Header Header { get; set; }

    [JsonProperty("endpoint")]
    public Endpoint Endpoint { get; set; }

    [JsonProperty("payload")]
    public T Payload { get; set; }
}

public class Directive : Directive<Payload> { }