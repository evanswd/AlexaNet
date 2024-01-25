using Alexa.NET.Skills.Monoprice.Service;
using Alexa.NET.SmartHome.Attributes;
using Alexa.NET.SmartHome.Domain;
using Alexa.NET.SmartHome.Domain.Constants;
using Alexa.NET.SmartHome.Domain.Payloads;
using Alexa.NET.SmartHome.Domain.Request;
using Alexa.NET.SmartHome.Domain.Response;
using Alexa.NET.SmartHome.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;

namespace Alexa.NET.Skills.Monoprice.Endpoints;

[RequiresLock]
public class SpeakerZone : AbstractSmartHomeInterface, IDiscovery, IReportState,
    IPowerController, ISpeaker, IEqualizerController
{
    private MonopriceService _mpService;

    private MonopriceService MonopriceService
    {
        get
        {
            return _mpService ??= new MonopriceService(Config["Monoprice.IpAddress"],
                int.Parse(Config["Monoprice.TcpPortController1"]),
                int.Parse(Config["Monoprice.TcpPortController2"]));
        }
    }

    public SpeakerZone(IConfiguration config, string alexaNamespace)
        : base(config, alexaNamespace) { }

    #region IPowerController Handlers

    public EventResponse TurnOn(DirectiveRequest dr)
    {
        using (MonopriceService)
        {
            MonopriceService.SetPowerOn(dr.Directive.Endpoint.EndpointID);
            return BuildResponse(dr.Directive);
        }
    }

    public EventResponse TurnOff(DirectiveRequest dr)
    {
        using (MonopriceService)
        {
            MonopriceService.SetPowerOff(dr.Directive.Endpoint.EndpointID);
            return BuildResponse(dr.Directive);
        }
    }

    #endregion

    #region ISpeaker Handlers

    public EventResponse SetMute(DirectiveRequest<SetMutePayload> dr)
    {
        using (MonopriceService)
        {
            MonopriceService.SetMute(dr.Directive.Payload.Mute, dr.Directive.Endpoint.EndpointID);
            return BuildResponse(dr.Directive);
        }
    }

    public EventResponse SetVolume(DirectiveRequest<SetVolumePayload> dr)
    {
        using (MonopriceService)
        {
            var status = MonopriceService.GetStatus().Single(zs => zs.Name == dr.Directive.Endpoint.EndpointID);
            var volume = dr.Directive.Payload.Volume;

            //If it is off and we set it to any volume, turn it on...
            if (volume <= 0 && status.PowerOn)
                MonopriceService.SetPowerOff(dr.Directive.Endpoint.EndpointID);

            //If it is on and we set it to 0 or less... turn it off...
            if (volume > 0 && (!status.PowerOn || status.Volume <= 0))
                MonopriceService.SetPowerOn(dr.Directive.Endpoint.EndpointID);

            //When in doubt, change the volume...
            MonopriceService.SetVolume(ConvertVolumeToMonoprice(volume), dr.Directive.Endpoint.EndpointID);

            //Kick it back to Alexa
            return BuildResponse(dr.Directive);
        }
    }

    public EventResponse AdjustVolume(DirectiveRequest<AdjustVolumePayload> dr)
    {
        using (MonopriceService)
        {
            var status = MonopriceService.GetStatus().Single(zs => zs.Name == dr.Directive.Endpoint.EndpointID);
            var volume = dr.Directive.Payload.Volume;

            //Force the volume to be between 0 and 100 
            volume = Math.Max(Math.Min(ConvertVolumeToAlexa(status.Volume) + volume, 100), 0);

            //If it is off and we set it to any volume, turn it on...
            if (volume == 0 && status.PowerOn)
                MonopriceService.SetPowerOff(dr.Directive.Endpoint.EndpointID);

            //If it is on and we set it to 0 or less... turn it off...
            if (volume > 0 && !status.PowerOn)
                MonopriceService.SetPowerOn(dr.Directive.Endpoint.EndpointID);

            //When in doubt, change the volume...
            MonopriceService.SetVolume(ConvertVolumeToMonoprice(volume), dr.Directive.Endpoint.EndpointID);

            //Kick it back to Alexa
            return BuildResponse(dr.Directive);
        }
    }

    #endregion

    #region IEqualizerController Handlers

    public EventResponse SetMode(DirectiveRequest<SetModePayload> dr)
    {

        return BuildErrorResponse(dr.Directive, ErrorTypes.INVALID_VALUE,
            "The speaker does not support setting the mode.");
    }

    public EventResponse SetBands(DirectiveRequest<SetBandsPayload> dr)
    {
        if (dr.Directive.Payload.Bands.Any(b => b.Name == EqualizerBands.MIDRANGE))
            return BuildErrorResponse(dr.Directive, ErrorTypes.INVALID_VALUE,
                "The speaker does not support mid range.");

        using (MonopriceService)
        {
            foreach (var band in dr.Directive.Payload.Bands)
            {
                if (band.Name == EqualizerBands.BASS)
                    MonopriceService.SetBass(band.Value, dr.Directive.Endpoint.EndpointID);
                else if (band.Name == EqualizerBands.TREBLE)
                    MonopriceService.SetTreble(band.Value, dr.Directive.Endpoint.EndpointID);
            }

            return BuildResponse(dr.Directive);
        }
    }

    public EventResponse AdjustBands(DirectiveRequest<AdjustBandsPayload> dr)
    {
        if (dr.Directive.Payload.Bands.Any(b => b.Name == EqualizerBands.MIDRANGE))
            return BuildErrorResponse(dr.Directive, ErrorTypes.INVALID_VALUE,
                "The speaker does not support mid range.");

        using (MonopriceService)
        {
            var status = MonopriceService.GetStatus().Single(zs => zs.Name == dr.Directive.Endpoint.EndpointID);

            foreach (var band in dr.Directive.Payload.Bands)
            {
                //Why not just pass a negative number? adjusting for it...
                if (band.LevelDirection == LevelDirection.DOWN)
                    band.LevelDelta *= -1;

                if (band.Name == EqualizerBands.BASS)
                {
                    var bass = Math.Max(Math.Min(status.Bass + band.LevelDelta, 7), -7) + 7;
                    MonopriceService.SetBass(bass, dr.Directive.Endpoint.EndpointID);
                }
                else if (band.Name == EqualizerBands.TREBLE)
                {
                    var treble = Math.Max(Math.Min(status.Treble + band.LevelDelta, 7), -7) + 7;
                    MonopriceService.SetTreble(treble, dr.Directive.Endpoint.EndpointID);
                }
            }

            //Kick it back to Alexa
            return BuildResponse(dr.Directive);
        }
    }

    public EventResponse ResetBands(DirectiveRequest<SetBandsPayload> dr)
    {
        using (MonopriceService)
        {
            MonopriceService.SetBass(0, dr.Directive.Endpoint.EndpointID);
            MonopriceService.SetTreble(0, dr.Directive.Endpoint.EndpointID);
            return BuildResponse(dr.Directive);
        }
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
                    Endpoints = new[]
                    {
                        GenerateSpeakerEndpoint("Zone1","Kitchen Speakers"),
                        GenerateSpeakerEndpoint("Zone2","Dining Room Speakers"),
                        GenerateSpeakerEndpoint("Zone3","Office Speakers"),
                        GenerateSpeakerEndpoint("Zone4","Master Bedroom Speakers"),
                        GenerateSpeakerEndpoint("Zone5","Patio Speakers"),
                        GenerateSpeakerEndpoint("Zone6","Pool Speakers"),
                        GenerateSpeakerEndpoint("Zone7","Master Bathroom Speakers"),
                        GenerateSpeakerEndpoint("Zone8","Guest Bathroom Speakers")
                    }
                }
            }
        };

        //Fix the header...
        response.Event.Header.Name = "Discover.Response";
        //Kick it back
        return response;
    }

    #endregion

    #region IReportState Handlers

    public EventResponse ReportState(DirectiveRequest dr)
    {
        using (MonopriceService)
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

    #endregion

    private int ConvertVolumeToMonoprice(int alexaVolume)
    {
        //Alexa's volume is 0 to 100
        return (int)((alexaVolume / 100.0) * 38);
    }

    private int ConvertVolumeToAlexa(int monopriceVolume)
    {
        //Monoprice's volume is 0 to 38
        return (int)((monopriceVolume / 38.0) * 100);
    }

    private EventResponse BuildResponse<T>(Directive<T> directive) where T : Payload
    {
        var status = MonopriceService.GetStatus().Single(zs => zs.Name == directive.Endpoint.EndpointID);

        var properties = new[]
        {
            new ContextProperty {Namespace = "Alexa.PowerController", Name = "powerState", Value = status.PowerOn ? "ON" : "OFF"},
            new ContextProperty {Namespace = "Alexa.Speaker", Name = "volume", Value = ConvertVolumeToAlexa(status.Volume)},
            new ContextProperty {Namespace = "Alexa.Speaker", Name = "muted", Value = status.Muted},
            new ContextProperty {Namespace = "Alexa.EqualizerController", Name = "bands", Value = new[]
            {
                new { name = EqualizerBands.BASS, value = status.Bass },
                new { name = EqualizerBands.TREBLE, value = status.Treble }
            }}
        };

        return BuildResponse(directive, new Context { Properties = properties });
    }

    private Endpoint GenerateSpeakerEndpoint(string endpointID, string friendlyName)
    {
        return new Endpoint
        {
            EndpointID = endpointID, //Careful... alphanumeric only... BAD documentation...
            ManufacturerName = "Monoprice",
            Description = "Not-so-smart Speaker by Bill Evans",
            FriendlyName = friendlyName,
            DisplayCategories = new[] { DisplayCategories.SPEAKER },
            Capabilities = new[]
            {
                new Capability("Alexa"),
                new Capability("Alexa.Speaker", null, "volume", "muted"),
                new Capability("Alexa.PowerController", null, "powerState")
                //new Capability("Alexa.EqualizerController", new Configuration
                //{
                //    Bands = new EqualizerBand
                //    {
                //        Supported = new[] {
                //            new Supported(EqualizerBands.BASS.ToString()),
                //            new Supported(EqualizerBands.TREBLE.ToString())
                //        },
                //        Range = new EqualizerRange(-7, 7)

                //    }
                //}, "bands") 
            }
        };
    }
}