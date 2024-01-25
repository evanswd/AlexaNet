// Ignore Spelling: Insteon

using Alexa.NET.Skills.Insteon.Service.Models.Request;
using Alexa.NET.Skills.Insteon.Service.Models.Response;

namespace Alexa.NET.Skills.Insteon.Service;

public class InsteonService(string url)
{
    public async Task<StatusResponse> GetDeviceStatus(StatusRequest request)
    {
        var client = new HttpClient();
        var result = await client.GetAsync(url + $"/sx.xml?{request.DeviceId}={request.StatusCommand}");
        return new StatusResponse(await result.Content.ReadAsStringAsync());
    }
}