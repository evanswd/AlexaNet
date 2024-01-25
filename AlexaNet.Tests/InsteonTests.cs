// Ignore Spelling: Insteon

using Alexa.NET.Skills.Insteon.Service;
using Alexa.NET.Skills.Insteon.Service.Models.Request;
using Microsoft.Extensions.Configuration;

namespace AlexaNet.Tests;

[TestClass]
public class InsteonTests
{
    private string _url = null!;

    [TestInitialize]
    public void Initialize()
    {
        _url = "http://192.168.1.136:25105";
    }

    [TestMethod]
    public void InsteonStatusTest()
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();

        var auth = $"{configuration["Authentication:Username"]}:{configuration["Authentication:Password"]}";

        var svc = new InsteonService(_url, auth);
        var status = svc.GetDeviceStatus(new StatusRequest("282C7A", "19")).Result;
        var percentage = (int)Math.Round(status.OnLevel / 255.0 * 100);
        Console.WriteLine("The light is currently at: " + percentage + "%");
    }
}