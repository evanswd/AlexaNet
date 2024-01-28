using Alexa.NET.Skills.Insteon.Service;
using Alexa.NET.Skills.Insteon.Service.Models;
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

    [TestMethod]
    public void TurnLightOnTest()
    {
        _svc.TurnLightOn("282C7A", 100).Wait();
    }

    [TestMethod]
    public void TurnLightOffTest()
    {
        _svc.TurnLightOff("282C7A").Wait();
    }

    [TestMethod]
    public void TurnFanOnTest()
    {
        _svc.TurnFanOn("282C7A", FanSpeed.HIGH).Wait();
    }

    [TestMethod]
    public void TurnFanOffTest()
    {
        _svc.TurnFanOff("282C7A").Wait();
    }

    [TestMethod]
    public void TestStatusLatency()
    {
        var timeout = 750; //0.5 seconds is the default for Insteon... so we wait for that and add a quarter of a second for good measure.
        var attempts = 20;
        var deviceId = "282C7A";

        for (var i = 0; i < attempts; i++)
        {
            _svc.TurnLightOn(deviceId, 100).Wait();
            Thread.Sleep(timeout);
            var status = _svc.GetLightStatus(deviceId).Result;
            Console.WriteLine(status);
            _svc.TurnLightOff(deviceId).Wait();
            Thread.Sleep(timeout);
            status = _svc.GetLightStatus(deviceId).Result;
            Console.WriteLine(status);
        }
    }
}