using System.Xml;

namespace Alexa.NET.Skills.Insteon.Service.Models.Response;

public class StatusResponse
{
    public string? DeviceId { get; private set; }
    public byte AckHops { get; private set; }
    public byte Delta { get; private set; }
    public byte OnLevel { get; private set; }

    public StatusResponse() { }

    public StatusResponse(string responseXML)
    {
        ParseResponseXML(responseXML);
    }

    public void ParseResponseXML(string responseXML)
    {
        try
        {
            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(responseXML);

            var xNode = xmlDoc.SelectSingleNode("//X");
            if (xNode?.Attributes?["D"] == null)
                throw new Exception("Unable to parse the responseXML.");

            var dValue = xNode.Attributes["D"]?.Value;

            //Parse into pieces... {6}[DeviceID]{2}[ACK/Hops]{2}[Delta]{2}[OnLevel]
            DeviceId = dValue?[..6] ?? throw new Exception("Unable to parse the DeviceId.");

            AckHops = Convert.ToByte(dValue.Substring(6, 2), 16);
            Delta = Convert.ToByte(dValue.Substring(8, 2), 16);
            OnLevel = Convert.ToByte(dValue.Substring(10, 2), 16);
        }
        catch (Exception e)
        {
            throw new Exception("Unable to parse the status response.", e);
        }
    }

    public override string ToString()
    {
        return $"Device ID '{DeviceId}' has a status of {AckHops:X2}{Delta:X2}{OnLevel:X2}";
    }
}
