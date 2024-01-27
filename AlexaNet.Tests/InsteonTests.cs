using Alexa.NET.Skills.Insteon.Service;
using Microsoft.Extensions.Configuration;

namespace AlexaNet.Tests;

[TestClass]
public class InsteonTests
{
    private InsteonService _svc = null!;

    [TestInitialize]
    public void Initialize()
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appSettings.json")
            .Build();

        var url = $"http://{configuration["Insteon:Host"]}:{configuration["Insteon:Port"]}";

        _svc = new InsteonService(url, configuration["Insteon:Username"]!, configuration["Insteon:Password"]!);
    }

    [TestMethod]
    public void GetLightStatusTest()
    {
        var status = _svc.GetLightStatus("282C7A").Result;
        Console.WriteLine(status);
    }

    [TestMethod]
    public void GetFanStatusTest()
    {
        //var status = _svc.GetDeviceStatus("282C7A", "1903").Result;
        var status = _svc.GetFanStatus("282C7A").Result;
        Console.WriteLine(status);
    }
}