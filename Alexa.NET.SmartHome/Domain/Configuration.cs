using Newtonsoft.Json;

namespace Alexa.NET.SmartHome.Domain;

[JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
public class Configuration
{
    [JsonProperty("bands")]
    public EqualizerBand Bands { get; set; }

    [JsonProperty("modes")]
    public EqualizerMode Modes { get; set; }
}

public class EqualizerBand
{
    [JsonProperty("supported")]
    public Supported[] Supported { get; set; }

    [JsonProperty("range")]
    public EqualizerRange Range { get; set; }
}

public class EqualizerMode
{
    [JsonProperty("supported")]
    public Supported[] Supported { get; set; }
}

public class EqualizerRange
{
    [JsonProperty("minimum")]
    public int Minimum { get; set; }

    [JsonProperty("maximum")]
    public int Maximum { get; set; }

    public EqualizerRange() { }

    public EqualizerRange(int min, int max)
    {
        Minimum = min;
        Maximum = max;
    }
}