using Alexa.NET.SmartHome.Attributes;
using Alexa.NET.SmartHome.Domain.Payloads;
using Alexa.NET.SmartHome.Domain.Request;
using Alexa.NET.SmartHome.Domain.Response;

namespace Alexa.NET.SmartHome.Interfaces;

[AlexaNamespace("Alexa.PowerLevelController")]
public interface IPowerLevelController
{
    EventResponse SetPowerLevel(DirectiveRequest<SetPowerLevelPayload> directive);

    EventResponse AdjustPowerLevel(DirectiveRequest<AdjustPowerLevelPayload> directive);
}