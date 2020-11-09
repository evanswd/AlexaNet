using Newtonsoft.Json;

namespace Alexa.NET.SmartHome.Domain.Response
{
    public class EventResponse
    {
        [JsonProperty("event")] 
        public Directive Event { get; set; }

        [JsonProperty("context")] 
        public Context Context { get; set; }
    }
}
