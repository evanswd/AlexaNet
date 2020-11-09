using System.Linq;
using Newtonsoft.Json;

namespace Alexa.NET.SmartHome.Domain
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class Capability
    {
        [JsonProperty("type")]
        public string Type { get; private set; }

        [JsonProperty("interface")]
        public string Interface { get; set; }

        [JsonProperty("instance")]
        public string Instance { get; set; }

        [JsonProperty("version")]
        public string Version { get; private set; }

        [JsonProperty("properties")]
        public Properties Properties { get; set; }

        //[JsonProperty("capabilityResources")]
        //public CapabilityResources CapabilityResources { get; set; }

        //[JsonProperty("configuration")]
        //public Configuration Configuration { get; set; }

        //[JsonProperty("semantics")]
        //public Semantics Semantics { get; set; }

        //[JsonProperty("verificationsRequired")]
        //public VerificationsRequired[] VerificationsRequired { get; set; }

        public Capability(string alexaInterface, params string[] supported)
        {
            Type = "AlexaInterface";
            Version = "3";
            Interface = alexaInterface;
            if(supported != null && supported.Length > 0)
                Properties = new Properties
                { 
                    Supported = supported.Select(sup => new Supported(sup)).ToArray(),
                    Retrievable = true,
                    ProactivelyReported = true
                };
        }
    }
}
