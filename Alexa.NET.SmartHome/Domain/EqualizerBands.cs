using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Alexa.NET.SmartHome.Domain
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum EqualizerBands
    {
        BASS,
        MIDRANGE,
        TREBLE
    }
}
