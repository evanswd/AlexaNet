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
public class InsteonEndpoint : AbstractSmartHomeInterface, IDiscovery, IReportState, IPowerController
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
        //If it ends with FAN its a fan....
        if (directive.Endpoint.EndpointID.EndsWith("FAN"))
            status = InsteonService.GetFanStatus(directive.Endpoint.EndpointID[..^3]).Result;
        else
            status = InsteonService.GetLightStatus(directive.Endpoint.EndpointID).Result;

        var properties = new[]
        {
            new ContextProperty {Namespace = "Alexa.PowerController", Name = "powerState", Value = status.OnLevel > 0 ? "ON" : "OFF"},
            new ContextProperty {Namespace = "Alexa.PowerLevelController", Name = "powerLevel", Value = status.OnLevel * 100 / 255}
            //TODO: Add Fan State
        };

        return BuildResponse(directive, new Context { Properties = properties });
    }

    #endregion

    #region IDiscovery Handlers

    public EventResponse Discover(DirectiveRequest dr)
    {
        var response = new EventResponse
        {
            Event = new Directive
            {
                Header = dr.Directive.Header,
                Payload = new Payload
                {
                    //TODO: I would love to pull these from Insteon, but alas, I am lazy...
                    Endpoints = GenerateFanLincEndpoints("HEX_MB", "Master Fan")
                                .Concat(GenerateFanLincEndpoints("HEX_GB", "Guest Room Fan"))
                                .Concat(GenerateFanLincEndpoints("HEX_FB", "Front Bedroom Fan"))
                                .Concat(GenerateFanLincEndpoints("HEX_RB", "Rear Bedroom Fan")).ToArray()
                }
            }
        };

        //Fix the header...
        response.Event.Header.Name = "Discover.Response";
        //Kick it back
        return response;
    }

    #endregion

    #region IPowerController Handlers

    public EventResponse TurnOn(DirectiveRequest dr)
    {
        using (InsteonService)
        {
            if (dr.Directive.Endpoint.EndpointID.EndsWith("FAN"))
                //TODO: Should be Async
                InsteonService.TurnFanOn(dr.Directive.Endpoint.EndpointID[..^3]).RunSynchronously();
            else
                InsteonService.TurnLightOn(dr.Directive.Endpoint.EndpointID).RunSynchronously();

            return BuildResponse(dr.Directive);
        }
    }

    public EventResponse TurnOff(DirectiveRequest dr)
    {
        using (InsteonService)
        {
            if (dr.Directive.Endpoint.EndpointID.EndsWith("FAN"))
                //TODO: Should be Async
                InsteonService.TurnFanOff(dr.Directive.Endpoint.EndpointID[..^3]).RunSynchronously();
            else
                InsteonService.TurnLightOff(dr.Directive.Endpoint.EndpointID).RunSynchronously();

            return BuildResponse(dr.Directive);
        }
    }

    #endregion

    private Endpoint[] GenerateFanLincEndpoints(string endpointID, string friendlyName)
    {
        return
        [
            new Endpoint
            {
                //Add FAN to the endpoint cause Insteon uses the same device ID
                EndpointID = endpointID + "FAN", //Careful... alphanumeric only... BAD documentation...
                ManufacturerName = "Insteon",
                Description = "Not-so-smart FanLinc by Bill Evans",
                FriendlyName = friendlyName,
                DisplayCategories = [DisplayCategories.FAN],
                Capabilities =
                [
                    new Capability("Alexa"),
                    new Capability("Alexa.PowerController", null, "powerState"),
                    new Capability("Alexa.PowerLevelController", null, "powerLevel"),
                    //Fan State
                    new Capability("Alexa.RangeController", null, "rangeValue")
                    {
                        Instance = "Fan.Speed",
                        Configurations = new RangeConfiguration
                        {
                            SupportedRange = new SupportedRange
                            {
                                MaximumValue = 3,
                                MinimumValue = 0,
                                Precision = 1
                            },
                            Presets =
                            [
                                new RangePreset
                                {
                                    RangeValue = 0,
                                    PresetResources =
                                    [
                                        new PresetResource
                                        {
                                            FriendlyNames =
                                            [
                                                new FriendlyName("Alexa.Setting.FanSpeed.Off")
                                            ]
                                        }
                                    ]
                                },
                                new RangePreset
                                {
                                    RangeValue = 1,
                                    PresetResources =
                                    [
                                        new PresetResource
                                        {
                                            FriendlyNames =
                                            [
                                                new FriendlyName("Alexa.Setting.FanSpeed.Low")
                                            ]
                                        }
                                    ]
                                },
                                new RangePreset
                                {
                                    RangeValue = 2,
                                    PresetResources =
                                    [
                                        new PresetResource
                                        {
                                            FriendlyNames =
                                            [
                                                new FriendlyName("Alexa.Setting.FanSpeed.Medium")
                                            ]
                                        }
                                    ]
                                },
                                new RangePreset
                                {
                                    RangeValue = 3,
                                    PresetResources =
                                    [
                                        new PresetResource
                                        {
                                            FriendlyNames =
                                            [
                                                new FriendlyName("Alexa.Setting.FanSpeed.High")
                                            ]
                                        }
                                    ]
                                }
                            ]
                        }
                    }
                ]
            },
            new Endpoint
            {
                EndpointID = endpointID, //Careful... alphanumeric only... BAD documentation...
                ManufacturerName = "Insteon",
                Description = "Not-so-smart FanLinc by Bill Evans",
                FriendlyName = friendlyName + " Light",
                DisplayCategories = [DisplayCategories.LIGHT],
                Capabilities =
                [
                    new Capability("Alexa"),
                    new Capability("Alexa.PowerController", null, "powerState"),
                    new Capability("Alexa.PowerLevelController", null, "powerLevel")
                ]
            },
        ];
    }
}