using Newtonsoft.Json;

namespace Alexa.NET.SmartHome.Domain;

public class Supported
{
    [JsonProperty("name")]
    public string Name { get; set; }

    public Supported(string name)
    {
        Name = name;
    }
}