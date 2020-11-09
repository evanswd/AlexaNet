using Newtonsoft.Json;

namespace Alexa.NET.SmartHome.Domain.Response
{
    public class EventResponse
    {
        [JsonProperty("event")] 
        public Directive Event { get; set; }
    }
}
