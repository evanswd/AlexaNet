using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Alexa.NET.SmartHome.Domain.Constants;

[JsonConverter(typeof(StringEnumConverter))]
public enum LevelDirection
{
    DOWN,
    UP
}