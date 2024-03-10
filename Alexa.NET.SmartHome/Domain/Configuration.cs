using Newtonsoft.Json;

namespace Alexa.NET.SmartHome.Domain;

[JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
public abstract class Configuration
{
}

#region Range Configuration

public class RangeConfiguration : Configuration
{
    [JsonProperty("supportedRange")]
    public SupportedRange SupportedRange { get; set; }

    [JsonProperty("presets")]
    public RangePreset[] Presets { get; set; }
}

public class SupportedRange
{
    [JsonProperty("minimumValue")]
    public int MinimumValue { get; set; }

    [JsonProperty("maximumValue")]
    public int MaximumValue { get; set; }

    [JsonProperty("precision")]
    public int Precision { get; set; }
}

public class RangePreset
{
    [JsonProperty("rangeValue")]
    public int RangeValue { get; set; }

    [JsonProperty("presetResources")]
    public PresetResource[] PresetResources { get; set; }
}

public class PresetResource
{
    [JsonProperty("friendlyNames")]
    public FriendlyName[] FriendlyNames { get; set; }
}

public class FriendlyName
{
    [JsonProperty("type")]
    public string Type { get; set; }

    [JsonProperty("value")]
    public FriendlyNameValue Value { get; set; }

    public FriendlyName() { }

    public FriendlyName(string assetID)
    {
        Type = "asset";
        Value = new FriendlyNameValue { AssetID = assetID };
    }

    public FriendlyName(string text, string locale)
    {
        Type = "text";
        Value = new FriendlyNameValue { Text = text, Locale = locale ?? "en-US" };
    }

    public class FriendlyNameValue
    {
        [JsonProperty("assetId")]
        public string AssetID { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("locale")]
        public string Locale { get; set; }
    }
}

#endregion

#region Equalizer Configuration

public class EqualizerConfiguration : Configuration
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

#endregion