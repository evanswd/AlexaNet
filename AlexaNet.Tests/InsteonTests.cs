using Alexa.NET.Skills.Insteon.Service;
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
            .AddJsonFile("appSettings.json")
            .Build();

        var svc = new InsteonService(_url, configuration["Authentication:Username"]!, configuration["Authentication:Password"]!);
        var status = svc.GetLightStatus("282C7A").Result;
        Console.WriteLine(status);
    }
}