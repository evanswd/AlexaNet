using Alexa.NET.SmartHome.Domain;
using Alexa.NET.SmartHome.Domain.Response;
using Microsoft.Extensions.Configuration;

namespace Alexa.NET.SmartHome.Interfaces
{
    [AlexaNamespace("Alexa.PowerController")]
    public interface IPowerController
    {
        EventResponse TurnOn(Directive directive);

        EventResponse TurnOff(Directive directive);
    }
}
