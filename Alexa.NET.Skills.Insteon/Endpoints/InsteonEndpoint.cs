using Alexa.NET.Skills.Insteon.Service;
using Alexa.NET.Skills.Insteon.Service.Models.Response;
using Alexa.NET.SmartHome.Attributes;
using Alexa.NET.SmartHome.Domain;
using Alexa.NET.SmartHome.Domain.Constants;
using Alexa.NET.SmartHome.Domain.Payloads;
using Alexa.NET.SmartHome.Domain.Request;
using Alexa.NET.SmartHome.Domain.Response;
using Alexa.NET.SmartHome.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Alexa.NET.Skills.Insteon.Endpoints;

[RequiresLock]
public class InsteonEndpoint : AbstractSmartHomeInterface, IReportState
{

    private InsteonService? _insteonService;

    private InsteonService InsteonService
    {
        get
        {
            return _insteonService ??= new InsteonService($"{Config["Insteon.Host"]}:{Config["Insteon.Port"]}",
                Config["Insteon.Username"], Config["Insteon.Password"]);
        }
    }
    public InsteonEndpoint(IConfiguration config, string alexaNamespace) 
        : base(config, alexaNamespace) { }


    #region IReportState Handlers

    public EventResponse ReportState(DirectiveRequest dr)
    {
        using (InsteonService)
        {
            var response = BuildResponse(dr.Directive);
            //Ditch the payload
            response.Event.Payload = null;
            //Fix the header...
            response.Event.Header.Name = "StateReport";
            //Kick it back
            return response;
        }
    }

    private EventResponse BuildResponse<T>(Directive<T> directive) where T : Payload
    {
        //TODO: Should be Async...
        StatusResponse status;
        //If it ends with 03 its a fan....
        if (directive.Endpoint.EndpointID.EndsWith("03"))
            status = InsteonService.GetFanStatus(directive.Endpoint.EndpointID[..^2]).Result;
        else
            status = InsteonService.GetLightStatus(directive.Endpoint.EndpointID[..^2]).Result;

        var properties = new[]
        {
            new ContextProperty {Namespace = "Alexa.PowerController", Name = "powerState", Value = status.OnLevel > 0 ? "ON" : "OFF"},
            new ContextProperty {Namespace = "Alexa.PowerLevelController", Name = "powerLevel", Value = status.OnLevel * 100 / 255}
        };

        return BuildResponse(directive, new Context { Properties = properties });
    }

    #endregion
}