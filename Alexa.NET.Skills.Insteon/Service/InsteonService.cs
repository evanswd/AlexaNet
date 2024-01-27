using System.Net.Http.Headers;
using System.Text;
using Alexa.NET.Skills.Insteon.Service.Models.Request;
using Alexa.NET.Skills.Insteon.Service.Models.Response;

namespace Alexa.NET.Skills.Insteon.Service;

public class InsteonService(string url, string username, string password)
{
    #region Device Statuses

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

    #endregion
}
