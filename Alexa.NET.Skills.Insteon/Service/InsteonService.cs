using System.Net.Http.Headers;
using System.Text;
using Alexa.NET.Skills.Insteon.Service.Models.Request;
using Alexa.NET.Skills.Insteon.Service.Models.Response;

namespace Alexa.NET.Skills.Insteon.Service;

public class InsteonService(string url, string username, string password)
{
    #region Device Statuses

    public async Task<StatusResponse> GetDeviceStatus(string deviceId, string statusCommand = "1900")
    {
        return await GetDeviceStatus(new StatusRequest(deviceId, statusCommand));
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

        var result = await client.GetAsync(url + $"/sx.xml?{request.DeviceId}={request.StatusCommand}");
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
        //1903 is the specific status command for fans
        return await GetDeviceStatus<FanStatusResponse>(new StatusRequest(deviceId, "1903"));
    }

    #endregion

    #region Device Commands

    public async Task SendDeviceCommand(string deviceId, byte cmd1, byte cmd2)
    {
        await SendDeviceCommand(new CommandRequest(deviceId, cmd1, cmd2));
    }

    public async Task SendDeviceCommand(CommandRequest request)
    {
        var client = new HttpClient();

        // Add Basic Authentication headers
        var authHeader = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes($"{username}:{password}")));
        client.DefaultRequestHeaders.Authorization = authHeader;

        var cmdUrl = url + $"/3?0262{request.DeviceId}{request.MsgFlag:X2}{request.Cmd1:X2}{request.Cmd2:X2}";
        if (request.ExtendedData != null)
        {
            cmdUrl += BitConverter.ToString(request.ExtendedData).Replace("-", "");
        }
        cmdUrl += "=I=3";

        await client.GetAsync(cmdUrl);
    }

    #endregion
}
