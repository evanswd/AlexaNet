using System;
using Newtonsoft.Json;

namespace Alexa.NET.SmartHome.Domain;

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
    public object Value { get; set; }

    [JsonProperty("timeOfSample")]
    public string TimeOfSample { get; set; }

    [JsonProperty("uncertaintyInMilliseconds")]
    public int UncertaintyInMilliseconds { get; set; }

    public ContextProperty()
    {
            TimeOfSample = DateTime.UtcNow.ToString("s") + "Z";
            UncertaintyInMilliseconds = 500;
        }
}