using System;
using AlexaNet.Infrastructure.Services.Utilities;

namespace AlexaNet.Infrastructure.Services.Denon
{
    public class HeosService : IService
    {
        private readonly TcpPortConnection _conn;

        public HeosService(string ipAddress)
        {
            //Heos uses telnet (23)
            _conn = new TcpPortConnection(ipAddress, 23);
            _conn.OpenConnection();
        }

        public void Dispose()
        {
            _conn.Dispose();
        }

        private void SendData(string data)
        {
            _conn.WriteData(data);
        }

        public void SetPowerOn()
        {
            SendData("PWON");
        }

        public void SetPowerOff()
        {
            SendData("PWSTANDBY");
        }

        public void SetVolume(int volume)
        {
            if (volume < 0 || volume > 80)
                throw new ArgumentOutOfRangeException(nameof(volume), "The volume must be between 0 and 80.");

            SendData($"MV{volume}");
        }

        public void SetSource(Sources source)
        {
            var sourceStr = source.ToString();
            sourceStr = sourceStr.Replace("__", "/").Replace("_", " ");

            SendData($"SI{sourceStr}");
        }
    }
}
