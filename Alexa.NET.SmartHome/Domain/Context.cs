using Newtonsoft.Json;

namespace Alexa.NET.SmartHome.Domain
{
    public class Context
    {
        [JsonProperty("properties")] 
        public ContextProperty[] Properties { get; set; }
    }

    public class ContextProperty
    {
        [JsonProperty("namespace")]
        public string Namespace { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }

        [JsonProperty("timeOfSample")]
        public string TimeOfSample { get; set; }

        [JsonProperty("uncertaintyInMilliseconds")]
        public int UncertaintyInMilliseconds { get; set; }
    }
}
