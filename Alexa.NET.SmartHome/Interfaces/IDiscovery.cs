using Alexa.NET.SmartHome.Attributes;
using Alexa.NET.SmartHome.Domain.Request;
using Alexa.NET.SmartHome.Domain.Response;

namespace Alexa.NET.SmartHome.Interfaces
{
    [AlexaNamespace("Alexa.Discovery")]
    public interface IDiscovery
    {
        EventResponse Discover(DirectiveRequest directive);
    }
}
