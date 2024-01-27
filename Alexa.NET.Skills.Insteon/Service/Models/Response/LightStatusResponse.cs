namespace Alexa.NET.Skills.Insteon.Service.Models.Response
{
    public class LightStatusResponse : StatusResponse
    {
        public bool IsOn => OnLevel > 0;

        public double OnLevelPct => LevelToPct(OnLevel);

        public override string ToString()
        {
            return $"Light ID '{DeviceId}' is " + (!IsOn ? "OFF" : $"ON at {OnLevelPct:00%}");
        }

        public static byte PctToLevel(double pct)
            => (byte)(Math.Max(0, Math.Min(1, pct)) * byte.MaxValue);

        public static double LevelToPct(byte level)
            => level / (double)byte.MaxValue;
    }
}
