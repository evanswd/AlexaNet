using Alexa.NET.SmartHome.Domain.Payloads;
using Newtonsoft.Json;

namespace Alexa.NET.SmartHome.Domain.Request;

public class DirectiveRequest<T> where T : Payload
{
    [JsonProperty("directive")] 
    public Directive<T> Directive { get; set; }
}

public class DirectiveRequest : DirectiveRequest<Payload> { }