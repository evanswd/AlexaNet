using Alexa.NET.SmartHome.Domain;
using Alexa.NET.SmartHome.Domain.Response;

namespace Alexa.NET.SmartHome.Interfaces
{
    [AlexaNamespace("Alexa.Speaker")]
    public interface ISpeaker
    {
        EventResponse SetMute(Directive directive);

        EventResponse SetVolume(Directive directive);

        EventResponse AdjustVolume(Directive directive);
    }
}
