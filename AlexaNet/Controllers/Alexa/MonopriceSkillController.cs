using System.Threading.Tasks;
using Alexa.NET.SmartHome.Domain.Request;
using Alexa.NET.SmartHome.Domain.Response;
using Alexa.NET.SmartHome.IoC;
using Microsoft.AspNetCore.Mvc;
using AlexaNet.Infrastructure.Amazon.Alexa;
using Microsoft.Extensions.Configuration;

namespace AlexaNet.Controllers.Alexa
{
    [Route("Alexa/[controller]")]
    [ApiController]
    public class MonopriceSkillController : ControllerBase
    {
        private IConfiguration _config;

        public MonopriceSkillController(IConfiguration config)
        {
            _config = config;
        }

        [HttpPost]
        public async Task<EventResponse> Post(DirectiveRequest request)
        {
            //Gotta decide what we are doing here...
            if(request?.Directive?.Header?.Namespace =="Alexa.Discovery" && request.Directive?.Header?.Name == "Discover")
                return DiscoveryUtils.PerformDiscovery(request.Directive);

            return Invoker.InvokeAlexaMethod<EventResponse>(_config, request?.Directive?.Header, request?.Directive);

            /*if (request?.Directive?.Header?.Namespace == "Alexa.PowerController" && request.Directive?.Header?.Name == "TurnOff")
            {
                using var svc = new MonopriceService(_config["Monoprice.IpAddress"], int.Parse(_config["Monoprice.TcpPort"]));
                svc.SetPowerOff(3);

                var response = new EventResponse
                {
                    Event = new Directive
                    {
                        Header = request.Directive.Header,
                        Endpoint = new Endpoint
                        {
                            EndpointID = request.Directive.Endpoint.EndpointID
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
                                Value = "OFF",
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

            if (request?.Directive?.Header?.Namespace == "Alexa.PowerController" && request.Directive?.Header?.Name == "TurnOn")
            {
                using var svc = new MonopriceService(_config["Monoprice.IpAddress"], int.Parse(_config["Monoprice.TcpPort"]));
                svc.SetPowerOn(3);

                var response = new EventResponse
                {
                    Event = new Directive
                    {
                        Header = request.Directive.Header,
                        Endpoint = new Endpoint
                        {
                            EndpointID = request.Directive.Endpoint.EndpointID
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


            //If we get here, dump the input...
            using var reader = new StreamReader(Request.Body, Encoding.UTF8);
            var x = await reader.ReadToEndAsync();
            await System.IO.File.WriteAllTextAsync(DateTime.Now.Ticks + "_skill.log", x);
            
            return new EventResponse();*/


            /*if (input?.Directive?.Header?.Namespace == "Alexa.Discovery")
                return ResponseBuilder.Tell($"It's working baby! {input.Directive.Header.Name}");
            else return ResponseBuilder.Tell("It still works!");*/

            /*System.IO.File.WriteAllText(DateTime.Now.Ticks + "_skill.log", JsonConvert.SerializeObject(input.Directive));
            return "";*/
        }
    }
}
