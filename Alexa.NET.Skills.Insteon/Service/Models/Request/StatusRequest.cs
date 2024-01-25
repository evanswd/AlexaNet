namespace Alexa.NET.Skills.Insteon.Service.Models.Request;

public class StatusRequest(string deviceId, string statusCommand = "1900") 
    : AbstractDirectRequest(deviceId)
{
    public readonly string StatusCommand = statusCommand;
}