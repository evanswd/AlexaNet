// Ignore Spelling: Insteon

using System.Net.Http.Headers;
using System.Text;
using Alexa.NET.Skills.Insteon.Service.Models.Request;
using Alexa.NET.Skills.Insteon.Service.Models.Response;

namespace Alexa.NET.Skills.Insteon.Service;

public class InsteonService(string url, string basicAuth)
{
    public async Task<StatusResponse> GetDeviceStatus(StatusRequest request)
    {
        var client = new HttpClient();

        // Add Basic Authentication headers
        var authHeader = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes(basicAuth)));
        client.DefaultRequestHeaders.Authorization = authHeader;

        var result = await client.GetAsync(url + $"/sx.xml?{request.DeviceId}={request.StatusCommand}");
        return new StatusResponse(await result.Content.ReadAsStringAsync());
    }
}
