namespace Alexa.NET.Skills.Insteon.Service.Models.Request;

public class CommandRequest : AbstractDirectRequest
{
    public CommandRequest(string deviceId, byte cmd1, byte cmd2, byte[]? extendedData = null)
        : base(deviceId)
    {
        Cmd1 = cmd1;
        Cmd2 = cmd2;
        ExtendedData = extendedData;
    }

    public CommandRequest(string deviceId) 
        : base(deviceId) { }

    public byte Cmd1 { get; protected set; }
    public byte Cmd2 { get; protected set; }
    public byte[]? ExtendedData { get; protected set; }

    //0x1F is for an extended command; 0F is for a standard command.
    public byte MsgFlag => (byte)(ExtendedData == null ? 0x0F : 0x1F);
}