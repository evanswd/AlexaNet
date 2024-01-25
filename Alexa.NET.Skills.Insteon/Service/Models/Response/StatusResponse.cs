using System.Xml;

namespace Alexa.NET.Skills.Insteon.Service.Models.Response;

public class StatusResponse
{
    public string DeviceId { get; }
    public string AckHops { get; }
    public string Delta { get; }
    public string OnLevel { get; }

    public StatusResponse(string responseXML)
    {
        var xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(responseXML);

        var xNode = xmlDoc.SelectSingleNode("//X");
        if (xNode?.Attributes?["D"] == null)
        {
            throw new Exception("Unable to parse the responseXML.");
        }

        var dValue = xNode.Attributes["D"]?.Value;

        //Parse into pieces... {6}[DeviceID]{2}[ACK/Hops]{2}[Delta]{2}[OnLevel]
        DeviceId = dValue?[..6] ?? throw new Exception("Unable to parse the DeviceId.");
        AckHops = dValue.Substring(6, 2) ?? throw new Exception("Unable to parse the AckHops.");
        Delta = dValue.Substring(8, 2) ?? throw new Exception("Unable to parse the Delta.");
        OnLevel = dValue.Substring(10, 2) ?? throw new Exception("Unable to parse the OnLevel.");
    }
}
