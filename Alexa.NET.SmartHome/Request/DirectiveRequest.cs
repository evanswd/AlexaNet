using Alexa.NET.SmartHome.Domain;
using Newtonsoft.Json;

namespace Alexa.NET.SmartHome.Request
{
    public class DirectiveRequest
    {
        [JsonProperty("directive")] 
        public Directive Directive { get; set; }
    }
}
