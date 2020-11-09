namespace AlexaNet.Infrastructure.Services.Monoprice
{
    public class ZoneStatus
    {
        public string Name { get; set; }
        public bool PowerOn { get; set; }
        public bool KeypadConnected { get; set; }
        public int SelectedSource { get; set; }
        public int Volume { get; set; }
        public int Bass { get; set; }
        public int Treble { get; set; }

        public ZoneStatus() { }
        public ZoneStatus(string data)
        {
            Name = "Zone" + data.Substring(2, 1);
            PowerOn = data.Substring(6, 1) == "1";
            KeypadConnected = data.Substring(22, 1) == "1";
            SelectedSource = int.Parse(data.Substring(19, 2));
            Volume = (int.Parse(data.Substring(11, 2))/38)*100;
            Bass = int.Parse(data.Substring(15, 2)) - 7;
            Treble = int.Parse(data.Substring(13, 2)) - 7;
        }
    }
}
