using Alexa.NET.SmartHome.Domain;
using Alexa.NET.SmartHome.Domain.Response;

namespace Alexa.NET.SmartHome.Interfaces
{
    [AlexaNamespace("Alexa")]
    public interface IReportState
    {
        EventResponse ReportState(Directive directive);
    }
}
