using Alexa.NET.SmartHome.Attributes;
using Alexa.NET.SmartHome.Domain.Payloads;
using Alexa.NET.SmartHome.Domain.Request;
using Alexa.NET.SmartHome.Domain.Response;

namespace Alexa.NET.SmartHome.Interfaces;

[AlexaNamespace("Alexa.EqualizerController")]
public interface IEqualizerController
{
    EventResponse SetMode(DirectiveRequest<SetModePayload> directive);

    EventResponse SetBands(DirectiveRequest<SetBandsPayload> directive);

    EventResponse AdjustBands(DirectiveRequest<AdjustBandsPayload> directive);

    EventResponse ResetBands(DirectiveRequest<SetBandsPayload> directive);
}