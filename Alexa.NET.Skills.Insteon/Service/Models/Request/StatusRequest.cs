namespace Alexa.NET.Skills.Insteon.Service.Models.Request;

public class StatusRequest(string deviceId, byte statusCmd2 = 0x00) 
    : AbstractDirectRequest(deviceId)
{
    public readonly byte StatusCmd2 = statusCmd2;
}