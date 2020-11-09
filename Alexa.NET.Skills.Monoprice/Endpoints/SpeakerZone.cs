using System;
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

        public SpeakerZone(IConfiguration config) : base(config)
        {
            _monopriceService = new MonopriceService(config["Monoprice.IpAddress"], int.Parse(config["Monoprice.TcpPort"]));
        }

        private EventResponse CreateResponse(Directive directive)
        {
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
                Context = new Context
                {
                    Properties = new[]
                    {
                        new ContextProperty
                        {
                            Namespace = "Alexa.PowerController",
                            Name = "powerState",
                            Value = "ON",
                            TimeOfSample = DateTime.UtcNow.ToString("s") + "Z",
                            UncertaintyInMilliseconds = 500
                        }
                    }
                }
            };
            response.Event.Header.Namespace = "Alexa";
            response.Event.Header.Name = "Response";
            return response;
        }

        public EventResponse TurnOn(Directive directive)
        {
            _monopriceService.SetPowerOn(3);
            return CreateResponse(directive);
        }

        public EventResponse TurnOff(Directive directive)
        {
            _monopriceService.SetPowerOff(3);
            return CreateResponse(directive);
        }
    }
}
