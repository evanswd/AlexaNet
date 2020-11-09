using Alexa.NET.SmartHome.Domain;
using Alexa.NET.SmartHome.Domain.Response;

namespace Alexa.NET.SmartHome.Interfaces
{
    [AlexaNamespace("Alexa.Discovery")]
    public interface IDiscovery
    {
        EventResponse Discover(Directive directive);
    }
}
