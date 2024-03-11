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
public class InsteonEndpoint : AbstractSmartHomeInterface, IDiscovery, IReportState, IPowerController, IPowerLevelController
{

    private InsteonService? _insteonService;

    private InsteonService InsteonService
    {
        get
        {
            return _insteonService ??= new InsteonService($"http://{Config["Insteon:Host"]}:{Config["Insteon:Port"]}",
                Config["Insteon:Username"], Config["Insteon:Password"]);
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
                    Endpoints = GenerateFanLincEndpoints("282C7A", "Master Fan")
                                .Concat(GenerateFanLincEndpoints("282A1D", "Guest Room Fan"))
                                .Concat(GenerateFanLincEndpoints("2D7E22", "Front Bedroom Fan"))
                                .Concat(GenerateFanLincEndpoints("2D7CE0", "Rear Bedroom Fan")).ToArray()
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
                InsteonService.TurnFanOn(dr.Directive.Endpoint.EndpointID[..^3]).Wait();
            else
                InsteonService.TurnLightOn(dr.Directive.Endpoint.EndpointID).Wait();

            return BuildResponse(dr.Directive);
        }
    }

    public EventResponse TurnOff(DirectiveRequest dr)
    {
        using (InsteonService)
        {
            if (dr.Directive.Endpoint.EndpointID.EndsWith("FAN"))
                //TODO: Should be Async
                InsteonService.TurnFanOff(dr.Directive.Endpoint.EndpointID[..^3]).Wait();
            else
                InsteonService.TurnLightOff(dr.Directive.Endpoint.EndpointID).Wait();

            return BuildResponse(dr.Directive);
        }
    }

    #endregion

    #region IPowerLevelController Handlers

    public EventResponse SetPowerLevel(DirectiveRequest<SetPowerLevelPayload> dr)
    {
        using (InsteonService)
        {
            //Do nothing... Fans shouldn't have power levels...
            if (dr.Directive.Endpoint.EndpointID.EndsWith("FAN"))
                return BuildResponse(dr.Directive);

            if(dr.Directive.Payload.PowerLevel == 0)
                InsteonService.TurnLightOff(dr.Directive.Endpoint.EndpointID).Wait();
            else
                InsteonService.TurnLightOn(dr.Directive.Endpoint.EndpointID, dr.Directive.Payload.PowerLevel).Wait();

            return BuildResponse(dr.Directive);
        }
    }

    public EventResponse AdjustPowerLevel(DirectiveRequest<AdjustPowerLevelPayload> dr)
    {
        using (InsteonService)
        {
            //Do nothing... Fans shouldn't have power levels...
            if (dr.Directive.Endpoint.EndpointID.EndsWith("FAN"))
                return BuildResponse(dr.Directive);

            //For this one... we need to get the current level and adjust it...
            var status = InsteonService.GetLightStatus(dr.Directive.Endpoint.EndpointID).Result;
            var newLevel = status.OnLevel + dr.Directive.Payload.PowerLevelDelta;

            if (newLevel <= 0)
                InsteonService.TurnLightOff(dr.Directive.Endpoint.EndpointID).Wait();
            else
                InsteonService.TurnLightOn(dr.Directive.Endpoint.EndpointID, newLevel).Wait();

            return BuildResponse(dr.Directive);
        }
    }

    #endregion

    private Endpoint[] GenerateFanLincEndpoints(string endpointID, string friendlyName)
    {
        return
        [
            //TODO: Fan isn't working yet...
            //new Endpoint
            //{
            //    //Add FAN to the endpoint cause Insteon uses the same device ID
            //    EndpointID = endpointID + "FAN", //Careful... alphanumeric only... BAD documentation...
            //    ManufacturerName = "Insteon",
            //    Description = "Not-so-smart FanLinc by Bill Evans",
            //    FriendlyName = friendlyName,
            //    DisplayCategories = [DisplayCategories.FAN],
            //    Capabilities =
            //    [
            //        new Capability("Alexa"),
            //        new Capability("Alexa.PowerController", null, "powerState"),
            //        //new Capability("Alexa.PowerLevelController", null, "powerLevel"),
            //        //Fan State
            //        new Capability("Alexa.RangeController", null, "rangeValue")
            //        {
            //            Instance = "Fan.Speed",
            //            Configuration = new RangeConfiguration
            //            {
            //                SupportedRange = new SupportedRange
            //                {
            //                    MaximumValue = 3,
            //                    MinimumValue = 1,
            //                    Precision = 1
            //                },
            //                Presets =
            //                [
            //                    //new RangePreset
            //                    //{
            //                    //    RangeValue = 0,
            //                    //    PresetResources =
            //                    //    [
            //                    //        new PresetResource
            //                    //        {
            //                    //            FriendlyNames =
            //                    //            [
            //                    //                new FriendlyName("Off", null)
            //                    //            ]
            //                    //        }
            //                    //    ]
            //                    //},
            //                    new RangePreset
            //                    {
            //                        RangeValue = 1,
            //                        PresetResources =
            //                        [
            //                            new PresetResource
            //                            {
            //                                FriendlyNames =
            //                                [
            //                                    new FriendlyName("Alexa.Value.Low")
            //                                ]
            //                            }
            //                        ]
            //                    },
            //                    new RangePreset
            //                    {
            //                        RangeValue = 2,
            //                        PresetResources =
            //                        [
            //                            new PresetResource
            //                            {
            //                                FriendlyNames =
            //                                [
            //                                    new FriendlyName("Alexa.Value.Medium")
            //                                ]
            //                            }
            //                        ]
            //                    },
            //                    new RangePreset
            //                    {
            //                        RangeValue = 3,
            //                        PresetResources =
            //                        [
            //                            new PresetResource
            //                            {
            //                                FriendlyNames =
            //                                [
            //                                    new FriendlyName("Alexa.Value.High")
            //                                ]
            //                            }
            //                        ]
            //                    }
            //                ]
            //            }
            //        }
            //    ]
            //},
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