using System;
using System.Net.Http;

namespace AlexaNet.Services.Denon
{
    public class DenonService : IService
    {
        private readonly string _baseUrl;
        private static readonly HttpClient HttpClient = new HttpClient();

        public DenonService(string ipAddress)
        {
            _baseUrl = $"http://{ipAddress}/MainZone/index.put.asp?cmd0=";
        }

        public void Dispose() { }

        private async void SendRequest(string action)
        {
            await HttpClient.GetAsync(_baseUrl + action.Replace("/", "%2f").Replace(" ", "%20"));
        }

        public void PowerOn()
        {
            SendRequest("PutZone_OnOff/ON");
        }

        public void PowerOff()
        {
            SendRequest("PutZone_OnOff/OFF");
        }

        public void MuteOn()
        {
            SendRequest("PutVolumeMute/ON");
        }

        public void MuteOff()
        {
            SendRequest("PutVolumeMute/OFF");
        }

        public void SetVolume(int volume)
        {
            if(volume < 0 || volume > 80)
                throw new ArgumentOutOfRangeException(nameof(volume), "The volume must be between 0 and 80.");

            //The volume is actually a negative number offset from 80.
            volume -= 80;

            SendRequest($"PutMasterVolumeSet/{volume}");
        }

        public void SetSource(Sources source)
        {
            var sourceStr = source.ToString();
            sourceStr = sourceStr.Replace("__","/").Replace("_", " ");

            SendRequest($"PutZone_InputFunction/{sourceStr}");
        }
    }
}
