using System.Net.Http.Headers;
using System.Text;
using Alexa.NET.Skills.Insteon.Service.Models;
using Alexa.NET.Skills.Insteon.Service.Models.Request;
using Alexa.NET.Skills.Insteon.Service.Models.Response;

namespace Alexa.NET.Skills.Insteon.Service;

public class InsteonService(string url, string username, string password)
{
    private const byte StartByte = 0x02;
    private const byte SendInsteonMsg = 0x62;

    #region Device Statuses

    public async Task<StatusResponse> GetDeviceStatus(string deviceId, byte statusCmd2 = 0x00)
    {
        return await GetDeviceStatus(new StatusRequest(deviceId, statusCmd2));
    }

    public async Task<StatusResponse> GetDeviceStatus(StatusRequest request)
    {
        return await GetDeviceStatus<StatusResponse>(request);
    }
    public async Task<TResponse> GetDeviceStatus<TResponse>(StatusRequest request) where TResponse : StatusResponse, new()
    {
        var client = new HttpClient();

        // Add Basic Authentication headers
        var authHeader = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes($"{username}:{password}")));
        client.DefaultRequestHeaders.Authorization = authHeader;

        var statusUrl = url + $"/sx.xml?{request.DeviceId}={(byte)Command.STATUS_REQUEST:X2}{request.StatusCmd2}";
        var result = await client.GetAsync(statusUrl);
        var status = new TResponse();
        status.ParseResponseXML(await result.Content.ReadAsStringAsync());
        return status;
    }

    public async Task<LightStatusResponse> GetLightStatus(string deviceId)
    {
        return await GetDeviceStatus<LightStatusResponse>(new StatusRequest(deviceId));
    }

    public async Task<FanStatusResponse> GetFanStatus(string deviceId)
    {
        //0x03 is the specific status subcommand for fans
        return await GetDeviceStatus<FanStatusResponse>(new StatusRequest(deviceId, 0x03));
    }

    #endregion

    #region Device Commands

    public async Task SendDeviceCommand(string deviceId, Command cmd1, byte cmd2)
    {
        await SendDeviceCommand(new CommandRequest(deviceId, cmd1, cmd2));
    }

    public async Task SendDeviceCommand(CommandRequest request)
    {
        var client = new HttpClient();

        // Add Basic Authentication headers
        var authHeader = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes($"{username}:{password}")));
        client.DefaultRequestHeaders.Authorization = authHeader;

        var cmdUrl = url + $"/3?{StartByte:X2}{SendInsteonMsg:X2}{request.DeviceId}{request.MsgFlag:X2}{(byte)request.Cmd1:X2}{request.Cmd2:X2}";
        if (request.ExtendedData != null)
        {
            cmdUrl += BitConverter.ToString(request.ExtendedData).Replace("-", "");
        }
        cmdUrl += "=I=3";

        await client.GetAsync(cmdUrl);
    }

    public async Task TurnLightOn(string deviceId, double onLevelPct = 100)
    {
        await SendDeviceCommand(new CommandRequest(deviceId, Command.ON, (byte)(onLevelPct * 255 / 100)));
    }

    public async Task TurnLightOff(string deviceId)
    {
        await SendDeviceCommand(new CommandRequest(deviceId, Command.OFF, 0x00));
    }

    public async Task TurnFanOn(string deviceId, FanSpeed speed = FanSpeed.HIGH)
    {
        await SendDeviceCommand(new CommandRequest(deviceId, Command.ON, (byte)speed, [0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00]));
    }

    public async Task TurnFanOff(string deviceId)
    {
        await SendDeviceCommand(new CommandRequest(deviceId, Command.OFF, (byte)FanSpeed.OFF, [0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00]));
    }

    #endregion
}
