using Newtonsoft.Json;

namespace Alexa.NET.SmartHome.Domain
{
    public class Endpoint
    {
        //[JsonProperty("scope")]
        //public Scope Scope { get; set; }

        [JsonProperty("endpointId")] 
        public string EndpointID { get; set; }

        [JsonProperty("manufacturerName")]
        public string ManufacturerName { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("friendlyName")]
        public string FriendlyName { get; set; }

        [JsonProperty("displayCategories")]
        public DisplayCategories[] DisplayCategories { get; set; }

        [JsonProperty("additionalAttributes")]
        public AdditionalAttributes AdditionalAttributes { get; set; }

        [JsonProperty("capabilities")]
        public Capability[] Capabilities { get; set; }

        //[JsonProperty("connections")]
        //public string Connections { get; set; }

        //[JsonProperty("relationships")]
        //public string Relationships { get; set; }

        //[JsonProperty("cookie")] 
        //public KVPair[] Cookie { get; set; }
    }
}
