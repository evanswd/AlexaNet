using Alexa.NET.SmartHome.Domain.Constants;
using Newtonsoft.Json;

namespace Alexa.NET.SmartHome.Domain.Payloads;

public class ErrorPayload : Payload
{
    [JsonProperty("type")]
    public ErrorTypes Type { get; set; }

    [JsonProperty("message")]
    public string Message { get; set; }

    public ErrorPayload() { }

    public ErrorPayload(ErrorTypes type, string message)
    {
        Type = type;
        Message = message;
    }
}