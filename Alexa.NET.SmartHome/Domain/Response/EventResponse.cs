using Alexa.NET.SmartHome.Domain.Payloads;
using Newtonsoft.Json;

namespace Alexa.NET.SmartHome.Domain.Response
{
    public class EventResponse<T> where T : Payload
    {
        [JsonProperty("event")] 
        public Directive<T> Event { get; set; }

        [JsonProperty("context")] 
        public Context Context { get; set; }
    }

    public class EventResponse : EventResponse<Payload> { }
}
