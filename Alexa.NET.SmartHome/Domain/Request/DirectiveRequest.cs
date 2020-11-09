using Newtonsoft.Json;

namespace Alexa.NET.SmartHome.Domain.Request
{
    public class DirectiveRequest
    {
        [JsonProperty("directive")] 
        public Directive Directive { get; set; }
    }
}
