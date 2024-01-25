using Alexa.NET.SmartHome.Domain;
using Alexa.NET.SmartHome.Domain.Constants;
using Alexa.NET.SmartHome.Domain.Payloads;
using Alexa.NET.SmartHome.Domain.Response;
using Microsoft.Extensions.Configuration;

namespace Alexa.NET.SmartHome.Interfaces;

public abstract class AbstractSmartHomeInterface
{
    protected IConfiguration Config;
    protected readonly string AlexaNamespace;

    public AbstractSmartHomeInterface(IConfiguration config, string alexaNamespace)
    {
            Config = config;
            AlexaNamespace = alexaNamespace;
        }

    protected EventResponse BuildResponse<T>(Directive<T> directive, Context context) where T : Payload
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
                Context = context
            };
            response.Event.Header.Namespace = "Alexa";
            response.Event.Header.Name = "Response";
            return response;
        }

    protected EventResponse BuildErrorResponse<T>(Directive<T> directive, ErrorTypes errorType, string message) where T : Payload
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
                    Payload = new ErrorPayload(errorType, message)
                }
            };
            response.Event.Header.Namespace = "Alexa";
            response.Event.Header.Name = "ErrorResponse";
            return response;
        }
}