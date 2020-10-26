using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using AlexaNet.Services.Denon;
using AlexaNet.Services.Monoprice;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;

namespace AlexaNet.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IConfiguration _config;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
        }

        [HttpGet]
        [AllowAnonymous]
        public IEnumerable<WeatherForecast> Get()
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("SoundOff")]
        public void SoundOff()
        {
            using var svc = new HeosService("192.168.1.21");
            svc.SetPowerOff();

            //using var svc = new MonopriceService(_config["Monoprice.IpAddress"], int.Parse(_config["Monoprice.TcpPort"]));
            //svc.SetPowerOff(3);
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("SoundOn")]
        public void SoundOn()
        {
            using var svc = new HeosService("192.168.1.21");
            svc.SetPowerOn();
            
            //using var svc = new MonopriceService(_config["Monoprice.IpAddress"], int.Parse(_config["Monoprice.TcpPort"]));
            //svc.SetPowerOn(3);
        }
    }
}
