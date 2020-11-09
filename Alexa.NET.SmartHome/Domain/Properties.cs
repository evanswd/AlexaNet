using Newtonsoft.Json;

namespace Alexa.NET.SmartHome.Domain
{
    public class Properties
    {
        [JsonProperty("supported")]
        public Supported[] Supported { get; set; }

        [JsonProperty("proactivelyReported")]
        public bool ProactivelyReported { get; set; }

        [JsonProperty("retrievable")]
        public bool Retrievable { get; set; }
    }

    public class Supported
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        public Supported(string name)
        {
            Name = name;
        }
    }
}
