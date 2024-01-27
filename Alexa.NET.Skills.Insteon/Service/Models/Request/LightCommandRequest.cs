namespace Alexa.NET.Skills.Insteon.Service.Models.Request;

public class LightCommandRequest : CommandRequest
{
    public LightCommandRequest(string deviceId, double onLevelPct)
        : base(deviceId)
    {
        Cmd2 = (byte)(onLevelPct * 255 / 100);

        //TODO: Determine the Cmd1 value based on onLevelPct.
    }
}