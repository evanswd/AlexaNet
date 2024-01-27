namespace Alexa.NET.Skills.Insteon.Service.Models.Response
{
    public class FanStatusResponse : StatusResponse
    {
        public bool IsOn => OnLevel > 0;
        public FanSpeed FanSpeed => (FanSpeed)OnLevel;

        public override string ToString()
        {
            return $"FanLinc ID '{DeviceId}' fan is " + (!IsOn ? "OFF" : $"{FanSpeed}");
        }
    }
}
