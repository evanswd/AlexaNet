using System;
using System.Linq;
using Alexa.NET.Skills.Monoprice.Service;
using Alexa.NET.SmartHome.Domain;
using Alexa.NET.SmartHome.Domain.Response;
using Alexa.NET.SmartHome.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Alexa.NET.Skills.Monoprice.Endpoints
{
    public class SpeakerZone : AbstractSmartHomeInterface, IDiscovery, IReportState, IPowerController, ISpeaker
    {
        private readonly MonopriceService _monopriceService;

        public SpeakerZone(IConfiguration config, string alexaNamespace) 
            : base(config, alexaNamespace)
        {
            _monopriceService = new MonopriceService(config["Monoprice.IpAddress"], 
                int.Parse(config["Monoprice.TcpPort"]));
        }

        public EventResponse TurnOn(Directive directive)
        {
            using (_monopriceService)
            {
                _monopriceService.SetPowerOn(directive.Endpoint.EndpointID);
                return BuildResponse(directive);
            }
        }

        public EventResponse TurnOff(Directive directive)
        {
            using (_monopriceService)
            {
                _monopriceService.SetPowerOff(directive.Endpoint.EndpointID);
                return BuildResponse(directive);
            }
        }

        public EventResponse SetMute(Directive directive)
        {
            using (_monopriceService)
            {
                _monopriceService.SetMute(directive.Payload.Mute ?? false, directive.Endpoint.EndpointID);
                return BuildResponse(directive);
            }
        }

        public EventResponse SetVolume(Directive directive)
        {
            using (_monopriceService)
            {
                var status = _monopriceService.GetStatus().Single(zs => zs.Name == directive.Endpoint.EndpointID);
                var volume = directive.Payload.Volume;

                //If it is off and we set it to any volume, turn it on...
                if(volume <= 0 && status.PowerOn)
                    _monopriceService.SetPowerOff(directive.Endpoint.EndpointID);

                //If it is on and we set it to 0 or less... turn it off...
                if (volume > 0 && (!status.PowerOn || status.Volume <= 0))
                    _monopriceService.SetPowerOn(directive.Endpoint.EndpointID);

                //When in doubt, change the volume...
                _monopriceService.SetVolume(ConvertVolumeToMonoprice(volume ?? 0), directive.Endpoint.EndpointID);

                //Kick it back to Alexa
                return BuildResponse(directive);
            }
        }

        public EventResponse AdjustVolume(Directive directive)
        {
            using (_monopriceService)
            {
                var status = _monopriceService.GetStatus().Single(zs => zs.Name == directive.Endpoint.EndpointID);
                var volume = directive.Payload.Volume ?? 0;


                //If it is off and we set it to any volume, turn it on...
                if (volume <= 0 && status.PowerOn)
                    _monopriceService.SetPowerOff(directive.Endpoint.EndpointID);

                //If it is on and we set it to 0 or less... turn it off...
                if (volume > 0 && (!status.PowerOn || status.Volume <= 0))
                    _monopriceService.SetPowerOn(directive.Endpoint.EndpointID);

                //Force it to be between 0 and 100 
                volume = Math.Max(Math.Min(ConvertVolumeToAlexa(status.Volume) + volume, 100), 0);

                //When in doubt, change the volume...
                _monopriceService.SetVolume(ConvertVolumeToMonoprice(volume), directive.Endpoint.EndpointID);

                //Kick it back to Alexa
                return BuildResponse(directive);
            }
        }

        public EventResponse Discover(Directive directive)
        {
            var response = new EventResponse
            {
                Event = new Directive
                {
                    Header = directive.Header,
                    Payload = new Payload
                    {
                        Endpoints = new[]
                        {
                            GenerateSpeakerEndpoint("Zone1","Kitchen Speakers"),
                            GenerateSpeakerEndpoint("Zone2","Dining Room Speakers"),
                            GenerateSpeakerEndpoint("Zone3","Office Speakers"),
                            GenerateSpeakerEndpoint("Zone4","Master Bedroom Speakers"),
                            GenerateSpeakerEndpoint("Zone5","Patio Speakers"),
                            GenerateSpeakerEndpoint("Zone6","Pool Speakers")
                        }
                    }
                }
            };

            //Fix the header...
            response.Event.Header.Name = "Discover.Response";
            //Kick it back
            return response;
        }

        public EventResponse ReportState(Directive directive)
        {
            var response = BuildResponse(directive);
            //Ditch the payload
            response.Event.Payload = null;
            //Fix the header...
            response.Event.Header.Name = "StateReport";
            //Kick it back
            return response;
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

        private EventResponse BuildResponse(Directive directive)
        {
            var status =  _monopriceService.GetStatus().Single(zs => zs.Name == directive.Endpoint.EndpointID);

            var properties = new[]
            {
                new ContextProperty {Namespace = "Alexa.PowerController", Name = "powerState", Value = status.PowerOn ? "ON" : "OFF"},
                new ContextProperty {Namespace = "Alexa.Speaker", Name = "volume", Value = status.Volume.ToString()},
                new ContextProperty {Namespace = "Alexa.Speaker", Name = "muted", Value = status.Muted ? "true" : "false"}
            };
            
            var response = new EventResponse
            {
                Event = new Directive
                {
                    Header = directive.Header,
                    Endpoint = new Endpoint
                    {
                        EndpointID = directive.Endpoint.EndpointID
                    },
                    Payload = new Payload()
                },
                Context = new Context { Properties = properties }
            };
            response.Event.Header.Namespace = "Alexa";
            response.Event.Header.Name = "Response";
            return response;
        }

        private Endpoint GenerateSpeakerEndpoint(string endpointID, string friendlyName)
        {
            return new Endpoint
            {
                EndpointID = endpointID, //Careful... alphanumeric only... BAD documentation...
                ManufacturerName = "Monoprice",
                Description = "Not-so-smart Speaker by Bill Evans",
                FriendlyName = friendlyName,
                DisplayCategories = new[] {DisplayCategories.SPEAKER},
                Capabilities = new[]
                {
                    //new Capability("Alexa"), 
                    new Capability("Alexa.Speaker", null, "volume", "muted"),
                    new Capability("Alexa.PowerController", null, "powerState"),
                    new Capability("Alexa.EqualizerController", new Configuration
                    {
                        Bands = new EqualizerBand
                        {
                            Supported = new[] {
                                new Supported(EqualizerBands.BASS.ToString()),
                                new Supported(EqualizerBands.TREBLE.ToString())
                            },
                            Range = new EqualizerRange(-10, 10)

                        }
                    }, "bands") 
                }
            };
        }
    }
}
