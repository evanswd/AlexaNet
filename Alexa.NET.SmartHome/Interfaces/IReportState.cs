using Alexa.NET.SmartHome.Attributes;
using Alexa.NET.SmartHome.Domain.Request;
using Alexa.NET.SmartHome.Domain.Response;

namespace Alexa.NET.SmartHome.Interfaces
{
    [AlexaNamespace("Alexa")]
    public interface IReportState
    {
        EventResponse ReportState(DirectiveRequest directive);
    }
}
