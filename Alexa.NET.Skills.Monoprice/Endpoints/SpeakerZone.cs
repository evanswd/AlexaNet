using System;
using System.Linq;
using Alexa.NET.Skills.Monoprice.Service;
using Alexa.NET.SmartHome.Domain;
using Alexa.NET.SmartHome.Domain.Constants;
using Alexa.NET.SmartHome.Domain.Payloads;
using Alexa.NET.SmartHome.Domain.Request;
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

        #region IPowerController Handlers

        public EventResponse TurnOn(DirectiveRequest dr)
        {
            using (_monopriceService)
            {
                _monopriceService.SetPowerOn(dr.Directive.Endpoint.EndpointID);
                return BuildResponse(dr.Directive);
            }
        }

        public EventResponse TurnOff(DirectiveRequest dr)
        {
            using (_monopriceService)
            {
                _monopriceService.SetPowerOff(dr.Directive.Endpoint.EndpointID);
                return BuildResponse(dr.Directive);
            }
        }

        #endregion

        #region ISpeaker Handlers

        public EventResponse SetMute(DirectiveRequest<SetMutePayload> dr)
        {
            using (_monopriceService)
            {
                _monopriceService.SetMute(dr.Directive.Payload.Mute, dr.Directive.Endpoint.EndpointID);
                return BuildResponse(dr.Directive);
            }
        }

        public EventResponse SetVolume(DirectiveRequest<SetVolumePayload> dr)
        {
            using (_monopriceService)
            {
                var status = _monopriceService.GetStatus().Single(zs => zs.Name == dr.Directive.Endpoint.EndpointID);
                var volume = dr.Directive.Payload.Volume;

                //If it is off and we set it to any volume, turn it on...
                if (volume <= 0 && status.PowerOn)
                    _monopriceService.SetPowerOff(dr.Directive.Endpoint.EndpointID);

                //If it is on and we set it to 0 or less... turn it off...
                if (volume > 0 && (!status.PowerOn || status.Volume <= 0))
                    _monopriceService.SetPowerOn(dr.Directive.Endpoint.EndpointID);

                //When in doubt, change the volume...
                _monopriceService.SetVolume(ConvertVolumeToMonoprice(volume), dr.Directive.Endpoint.EndpointID);

                //Kick it back to Alexa
                return BuildResponse(dr.Directive);
            }
        }

        //Negative numbers are breaking this...
        public EventResponse AdjustVolume(DirectiveRequest<AdjustVolumePayload> dr)
        {
            using (_monopriceService)
            {
                var status = _monopriceService.GetStatus().Single(zs => zs.Name == dr.Directive.Endpoint.EndpointID);
                var volume = dr.Directive.Payload.Volume;

                //Force the volume to be between 0 and 100 
                volume = Math.Max(Math.Min(ConvertVolumeToAlexa(status.Volume) + volume, 100), 0);

                //If it is off and we set it to any volume, turn it on...
                if (volume == 0 && status.PowerOn)
                    _monopriceService.SetPowerOff(dr.Directive.Endpoint.EndpointID);

                //If it is on and we set it to 0 or less... turn it off...
                if (volume > 0 && !status.PowerOn)
                    _monopriceService.SetPowerOn(dr.Directive.Endpoint.EndpointID);

                //When in doubt, change the volume...
                _monopriceService.SetVolume(ConvertVolumeToMonoprice(volume), dr.Directive.Endpoint.EndpointID);

                //Kick it back to Alexa
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

        #endregion

        #region IReportState Handlers

        public EventResponse ReportState(DirectiveRequest dr)
        {
            var response = BuildResponse(dr.Directive);
            //Ditch the payload
            response.Event.Payload = null;
            //Fix the header...
            response.Event.Header.Name = "StateReport";
            //Kick it back
            return response;
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
            var status =  _monopriceService.GetStatus().Single(zs => zs.Name == directive.Endpoint.EndpointID);

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
                            Range = new EqualizerRange(-7, 7)

                        }
                    }, "bands") 
                }
            };
        }
    }
}
