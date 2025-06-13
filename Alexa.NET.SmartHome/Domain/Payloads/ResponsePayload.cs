using Newtonsoft.Json;

namespace Alexa.NET.SmartHome.Domain.Payloads;

public class ResponsePayload : Payload
{
    [JsonProperty("message")]
    public string Message { get; set; }
}