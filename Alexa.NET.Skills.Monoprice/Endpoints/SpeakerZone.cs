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
    private static MonopriceService _sharedMonopriceService;
    private static readonly object _serviceLock = new object();

    private MonopriceService MonopriceService
    {
        get
        {
            if (_sharedMonopriceService == null)
            {
                lock (_serviceLock)
                {
                    if (_sharedMonopriceService == null)
                    {
                        _sharedMonopriceService = new MonopriceService(
                            Config["Monoprice.IpAddress"],
                            null, // No logger for now to keep it simple
                            int.Parse(Config["Monoprice.TcpPortController1"]),
                            int.Parse(Config["Monoprice.TcpPortController2"]));
                    }
                }
            }
            return _sharedMonopriceService;
        }
    }

    public SpeakerZone(IConfiguration config, string alexaNamespace)
        : base(config, alexaNamespace) { }

    #region IPowerController Handlers

    public EventResponse TurnOn(DirectiveRequest dr)
    {
        var zoneName = ParseZoneName(dr.Directive.Endpoint.EndpointID);

        // Check current status first to avoid redundant commands
        var currentStatus = MonopriceService.GetStatus().SingleOrDefault(zs => zs.Name == zoneName);

        if (currentStatus != null && currentStatus.PowerOn)
        {
            // Already on - just return current state without executing command
            return BuildResponse(dr.Directive);
        }

        // Not on, so turn on
        MonopriceService.SetPowerOn(zoneName);

        // Return expected state (ON) rather than immediately querying hardware
        return BuildResponseWithExpectedState(dr.Directive, powerOn: true);
    }

    public EventResponse TurnOff(DirectiveRequest dr)
    {
        var zoneName = ParseZoneName(dr.Directive.Endpoint.EndpointID);

        // Check current status first to avoid redundant commands
        var currentStatus = MonopriceService.GetStatus().SingleOrDefault(zs => zs.Name == zoneName);

        if (currentStatus != null && !currentStatus.PowerOn)
        {
            // Already off - just return current state without executing command
            return BuildResponse(dr.Directive);
        }

        // Currently on, so turn off
        MonopriceService.SetPowerOff(zoneName);

        // Return expected state (OFF) rather than immediately querying hardware
        return BuildResponseWithExpectedState(dr.Directive, powerOn: false);
    }

    private EventResponse BuildResponseWithExpectedMuteState<T>(Directive<T> directive, bool muted) where T : Payload
    {
        var zoneName = ParseZoneName(directive.Endpoint.EndpointID);

        // Get current status for other properties (power, volume, etc.)
        var status = MonopriceService.GetStatus().Single(zs => zs.Name == zoneName);

        // Use expected mute state, but real values for other properties
        var properties = new[]
        {
            new ContextProperty {Namespace = "Alexa.PowerController", Name = "powerState", Value = status.PowerOn ? "ON" : "OFF"},
            new ContextProperty {Namespace = "Alexa.Speaker", Name = "volume", Value = ConvertVolumeToAlexa(status.Volume)},
            new ContextProperty {Namespace = "Alexa.Speaker", Name = "muted", Value = muted},
            new ContextProperty {Namespace = "Alexa.EqualizerController", Name = "bands", Value = new[]
            {
                new { name = EqualizerBands.BASS, value = status.Bass },
                new { name = EqualizerBands.TREBLE, value = status.Treble }
            }}
        };

        return BuildResponse(directive, new Context { Properties = properties });
    }

    #endregion

    #region ISpeaker Handlers

    public EventResponse SetMute(DirectiveRequest<SetMutePayload> dr)
    {
        var zoneName = ParseZoneName(dr.Directive.Endpoint.EndpointID);
        var requestedMute = dr.Directive.Payload.Mute;

        // Check current status first to avoid redundant commands
        var currentStatus = MonopriceService.GetStatus().SingleOrDefault(zs => zs.Name == zoneName);

        if (currentStatus != null && currentStatus.Muted == requestedMute)
        {
            // Already in requested state - just return current state
            return BuildResponse(dr.Directive);
        }

        MonopriceService.SetMute(requestedMute, zoneName);

        // Return expected mute state rather than immediately querying hardware
        return BuildResponseWithExpectedMuteState(dr.Directive, requestedMute);
    }

    public EventResponse SetVolume(DirectiveRequest<SetVolumePayload> dr)
    {
        var zoneName = ParseZoneName(dr.Directive.Endpoint.EndpointID);
        var status = MonopriceService.GetStatus().Single(zs => zs.Name == zoneName);
        var volume = dr.Directive.Payload.Volume;

        //If it is off and we set it to any volume, turn it on...
        if (volume <= 0 && status.PowerOn)
            MonopriceService.SetPowerOff(zoneName);

        //If it is on and we set it to 0 or less... turn it off...
        if (volume > 0 && (!status.PowerOn || status.Volume <= 0))
            MonopriceService.SetPowerOn(zoneName);

        //When in doubt, change the volume...
        MonopriceService.SetVolume(ConvertVolumeToMonoprice(volume), zoneName);

        return BuildResponse(dr.Directive);
    }

    public EventResponse AdjustVolume(DirectiveRequest<AdjustVolumePayload> dr)
    {
        var zoneName = ParseZoneName(dr.Directive.Endpoint.EndpointID);
        var status = MonopriceService.GetStatus().Single(zs => zs.Name == zoneName);
        var volume = dr.Directive.Payload.Volume;

        //Force the volume to be between 0 and 100 
        volume = Math.Max(Math.Min(ConvertVolumeToAlexa(status.Volume) + volume, 100), 0);

        //If it is off and we set it to any volume, turn it on...
        if (volume == 0 && status.PowerOn)
            MonopriceService.SetPowerOff(zoneName);

        //If it is on and we set it to 0 or less... turn it off...
        if (volume > 0 && !status.PowerOn)
            MonopriceService.SetPowerOn(zoneName);

        //When in doubt, change the volume...
        MonopriceService.SetVolume(ConvertVolumeToMonoprice(volume), zoneName);

        return BuildResponse(dr.Directive);
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

        foreach (var band in dr.Directive.Payload.Bands)
        {
            if (band.Name == EqualizerBands.BASS)
                MonopriceService.SetBass(band.Value, ParseZoneName(dr.Directive.Endpoint.EndpointID));
            else if (band.Name == EqualizerBands.TREBLE)
                MonopriceService.SetTreble(band.Value, ParseZoneName(dr.Directive.Endpoint.EndpointID));
        }

        return BuildResponse(dr.Directive);
    }

    public EventResponse AdjustBands(DirectiveRequest<AdjustBandsPayload> dr)
    {
        if (dr.Directive.Payload.Bands.Any(b => b.Name == EqualizerBands.MIDRANGE))
            return BuildErrorResponse(dr.Directive, ErrorTypes.INVALID_VALUE,
                "The speaker does not support mid range.");

        var status = MonopriceService.GetStatus().Single(zs => zs.Name == ParseZoneName(dr.Directive.Endpoint.EndpointID));

        foreach (var band in dr.Directive.Payload.Bands)
        {
            //Why not just pass a negative number? adjusting for it...
            if (band.LevelDirection == LevelDirection.DOWN)
                band.LevelDelta *= -1;

            if (band.Name == EqualizerBands.BASS)
            {
                var bass = Math.Max(Math.Min(status.Bass + band.LevelDelta, 7), -7) + 7;
                MonopriceService.SetBass(bass, ParseZoneName(dr.Directive.Endpoint.EndpointID));
            }
            else if (band.Name == EqualizerBands.TREBLE)
            {
                var treble = Math.Max(Math.Min(status.Treble + band.LevelDelta, 7), -7) + 7;
                MonopriceService.SetTreble(treble, ParseZoneName(dr.Directive.Endpoint.EndpointID));
            }
        }

        return BuildResponse(dr.Directive);
    }

    public EventResponse ResetBands(DirectiveRequest<SetBandsPayload> dr)
    {
        var zoneName = ParseZoneName(dr.Directive.Endpoint.EndpointID);
        MonopriceService.SetBass(0, zoneName);
        MonopriceService.SetTreble(0, zoneName);
        return BuildResponse(dr.Directive);
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
                        GenerateSpeakerEndpoint("MonopriceZone1","Kitchen Speakers"),
                        GenerateSpeakerEndpoint("MonopriceZone2","Dining Room Speakers"),
                        GenerateSpeakerEndpoint("MonopriceZone3","Office Speakers"),
                        GenerateSpeakerEndpoint("MonopriceZone4","Master Bedroom Speakers"),
                        GenerateSpeakerEndpoint("MonopriceZone5","Patio Speakers"),
                        GenerateSpeakerEndpoint("MonopriceZone6","Pool Speakers"),
                        GenerateSpeakerEndpoint("MonopriceZone7","Master Bathroom Speakers"),
                        GenerateSpeakerEndpoint("MonopriceZone8","Guest Bathroom Speakers")
                    }
                }
            }
        };

        //Fix the header...
        response.Event.Header.Name = "Discover.Response";
        return response;
    }

    #endregion

    #region IReportState Handlers

    public EventResponse ReportState(DirectiveRequest dr)
    {
        var response = BuildResponse(dr.Directive);
        //Ditch the payload
        response.Event.Payload = null;
        //Fix the header...
        response.Event.Header.Name = "StateReport";
        return response;
    }

    #endregion

    #region Helper Methods

    private string ParseZoneName(string endpointId)
    {
        // Convert MonopriceZone1 -> Zone1 for the MonopriceService
        if (endpointId.StartsWith("MonopriceZone"))
        {
            var zoneNumber = endpointId.Substring("MonopriceZone".Length);
            return $"Zone{zoneNumber}";
        }

        // Fallback for backward compatibility
        return endpointId;
    }

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
        var zoneName = ParseZoneName(directive.Endpoint.EndpointID);
        var status = MonopriceService.GetStatus().Single(zs => zs.Name == zoneName);

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

    private EventResponse BuildResponseWithExpectedState<T>(Directive<T> directive, bool powerOn) where T : Payload
    {
        var zoneName = ParseZoneName(directive.Endpoint.EndpointID);

        // Get current status for other properties (volume, mute, etc.)
        var status = MonopriceService.GetStatus().Single(zs => zs.Name == zoneName);

        // Use expected power state, but real values for other properties
        var properties = new[]
        {
            new ContextProperty {Namespace = "Alexa.PowerController", Name = "powerState", Value = powerOn ? "ON" : "OFF"},
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
            EndpointID = endpointID,
            ManufacturerName = "Monoprice",
            Description = "Smart Speaker Zone by Bill Evans",
            FriendlyName = friendlyName,
            DisplayCategories = new[] { DisplayCategories.SPEAKER },
            Capabilities = new[]
            {
                new Capability("Alexa"),
                new Capability("Alexa.Speaker", null, "volume", "muted"),
                new Capability("Alexa.PowerController", null, "powerState")
            }
        };
    }

    #endregion
}