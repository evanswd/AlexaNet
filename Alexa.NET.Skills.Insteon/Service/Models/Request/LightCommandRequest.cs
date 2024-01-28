namespace Alexa.NET.Skills.Insteon.Service.Models.Request;

public class LightCommandRequest : CommandRequest
{
    public LightCommandRequest(string deviceId, double onLevelPct)
        : base(deviceId)
    {
        Cmd1 = onLevelPct > 0 ? Command.ON : Command.OFF;
        Cmd2 = (byte)(onLevelPct * 255 / 100);
    }
}