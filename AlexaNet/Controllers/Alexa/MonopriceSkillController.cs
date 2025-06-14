﻿using Alexa.NET.SmartHome.Domain.Request;
using Alexa.NET.SmartHome.Domain.Response;
using Alexa.NET.SmartHome.IoC;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace AlexaNet.Controllers.Alexa;

[Route("Alexa/[controller]")]
[ApiController]
public class MonopriceSkillController : ControllerBase
{
    private readonly IConfiguration _config;
    private readonly ILogger<MonopriceSkillController> _logger;


    public MonopriceSkillController(IConfiguration config, ILogger<MonopriceSkillController> logger)
    {
        _config = config;
        _logger = logger;
    }

    [HttpPost]
    public async Task<EventResponse> Post()
    {
        using var reader = new StreamReader(Request.Body, Encoding.UTF8);
        var requestJson = await reader.ReadToEndAsync();
        //Debug Dump if needed:
        //await System.IO.File.WriteAllTextAsync(DateTime.Now.Ticks + "_monoprice_skill.log", requestJson);

        //Fetch the inbound request to parse it...
        var request = JsonConvert.DeserializeObject<DirectiveRequest>(requestJson);

        //Gotta decide what we are doing here...
        //TODO: I HATE using a string for the scope...
        return Invoker.InvokeAlexaMethod<EventResponse>(_config, request?.Directive?.Header, requestJson, _logger, "Alexa.NET.Skills.Monoprice");
    }
}