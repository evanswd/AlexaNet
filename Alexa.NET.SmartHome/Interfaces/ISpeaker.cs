using Alexa.NET.SmartHome.Attributes;
using Alexa.NET.SmartHome.Domain.Payloads;
using Alexa.NET.SmartHome.Domain.Request;
using Alexa.NET.SmartHome.Domain.Response;

namespace Alexa.NET.SmartHome.Interfaces
{
    [AlexaNamespace("Alexa.Speaker")]
    public interface ISpeaker
    {
        EventResponse SetMute(DirectiveRequest<SetMutePayload> directive);

        EventResponse SetVolume(DirectiveRequest<SetVolumePayload> directive);

        EventResponse AdjustVolume(DirectiveRequest<AdjustVolumePayload> directive);
    }
}
