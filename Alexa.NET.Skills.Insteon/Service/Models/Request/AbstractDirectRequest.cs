namespace Alexa.NET.Skills.Insteon.Service.Models.Request;

public abstract class AbstractDirectRequest(string deviceId)
{
    public readonly string DeviceId = deviceId;
}