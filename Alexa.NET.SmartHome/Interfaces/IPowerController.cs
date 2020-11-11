using Alexa.NET.SmartHome.Attributes;
using Alexa.NET.SmartHome.Domain.Request;
using Alexa.NET.SmartHome.Domain.Response;

namespace Alexa.NET.SmartHome.Interfaces
{
    [AlexaNamespace("Alexa.PowerController")]
    public interface IPowerController
    {
        EventResponse TurnOn(DirectiveRequest directive);

        EventResponse TurnOff(DirectiveRequest directive);
    }
}
