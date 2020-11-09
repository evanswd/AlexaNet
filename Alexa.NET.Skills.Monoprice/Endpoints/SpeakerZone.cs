using System.Linq;
using Alexa.NET.Skills.Monoprice.Service;
using Alexa.NET.SmartHome.Domain;
using Alexa.NET.SmartHome.Domain.Response;
using Alexa.NET.SmartHome.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Alexa.NET.Skills.Monoprice.Endpoints
{
    public class SpeakerZone : AbstractSmartHomeInterface, IPowerController
    {
        private readonly MonopriceService _monopriceService;

        public SpeakerZone(IConfiguration config, string alexaNamespace) 
            : base(config, alexaNamespace)
        {
            _monopriceService = new MonopriceService(config["Monoprice.IpAddress"], int.Parse(config["Monoprice.TcpPort"]));
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

        private EventResponse BuildResponse(Directive directive)
        {
            var status = _monopriceService.GetStatus().Single(zs => zs.Name == directive.Endpoint.EndpointID);

            var properties = new[]
            {
                new ContextProperty {Namespace = "Alexa.PowerController", Name = "powerState", Value = status.PowerOn ? "ON" : "OFF"},
                //new ContextProperty {Namespace = "Alexa.Speaker", Name = "volume", Value = status.Volume.ToString()},
                //TODO: This should work for real..
                //new ContextProperty {Namespace = "Alexa.Speaker", Name = "muted", Value = "false"}
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
    }
}
