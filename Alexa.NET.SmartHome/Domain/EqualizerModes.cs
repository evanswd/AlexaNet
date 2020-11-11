using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Alexa.NET.SmartHome.Domain
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum EqualizerModes
    {
        MOVIE,
        MUSIC,
        NIGHT,
        SPORT,
        TV
    }
}
