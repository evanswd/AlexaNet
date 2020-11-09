using System;
using System.Collections.Generic;
using System.Linq;
using Alexa.NET.SmartHome.Utilities;
using AlexaNet.Infrastructure.Services.Monoprice;

namespace Alexa.NET.Skills.Monoprice.Service
{
    public class MonopriceService : IDisposable
    {
        private readonly TcpPortConnection _conn;

        public MonopriceService(string ipAddress, int tcpPort)
        {
            _conn = new TcpPortConnection(ipAddress, tcpPort);
            _conn.OpenConnection();
        }

        public void Dispose()
        {
            _conn.Dispose();
        }


        public List<ZoneStatus> GetStatus()
        {
            var res = _conn.WriteData("?10", 6);
            var lines = res.Split(new[] {"\r\n"}, StringSplitOptions.None);
            return lines.Skip(1).Take(6).Select(line => new ZoneStatus(line)).ToList();
        }

        public void SetPowerOn(params int[] zones)
        {
            foreach (var zone in zones)
                _conn.WriteData($"<1{zone}PR01");
        }

        public void SetPowerOn(string zone)
        {
            SetPowerOn(int.Parse(zone.Substring(4)));
        }

        public void SetPowerOff(string zone)
        {
            SetPowerOff(int.Parse(zone.Substring(4)));
        }

        public void SetPowerOff(params int[] zones)
        {
            foreach (var zone in zones)
                _conn.WriteData($"<1{zone}PR00");
        }

        public void SetVolume(int volume, string zone)
        {
            SetVolume(volume, int.Parse(zone.Substring(4)));
        }

        public void SetVolume(int volume, params int[] zones)
        {
            if (volume < 0 || volume > 38)
                throw new ArgumentOutOfRangeException(nameof(volume), "The volume must be between 0 and 38.");

            var volumeStr = volume.ToString("D2");
            foreach (var zone in zones)
                _conn.WriteData($"<1{zone}VO{volumeStr}");
        }

        public void SetSource(int source, params int[] zones)
        {
            if (source < 1 || source > 6)
                throw new ArgumentOutOfRangeException(nameof(source), "The source must be between 1 and 6.");

            var sourceStr = source.ToString("D2");
            foreach (var zone in zones)
                _conn.WriteData($"<1{zone}CH{sourceStr}");
        }

        public void SetBalance(int balance, params int[] zones)
        {
            if(balance < 0 || balance > 20)
                throw new ArgumentOutOfRangeException(nameof(balance), "The balance must be between 0 and 20. (0-9 is Left, 10 is Center, and 11-20 is Right)");

            var balanceStr = balance.ToString("D2");
            foreach (var zone in zones)
                _conn.WriteData($"<1{zone}BL{balanceStr}");
        }

        public void SetBass(int bass, params int[] zones)
        {
            if (bass < 0 || bass > 14)
                throw new ArgumentOutOfRangeException(nameof(bass), "The bass must be between 0 and 14. (0-6 is Decrease, 7 is Flat, and 8-14 is Increase)");

            var balanceStr = bass.ToString("D2");
            foreach (var zone in zones)
                _conn.WriteData($"<1{zone}BS{balanceStr}");
        }

        public void SetTreble(int treble, params int[] zones)
        {
            if (treble < 0 || treble > 14)
                throw new ArgumentOutOfRangeException(nameof(treble), "The treble must be between 0 and 14. (0-6 is Decrease, 7 is Flat, and 8-14 is Increase)");

            var balanceStr = treble.ToString("D2");
            foreach (var zone in zones)
                _conn.WriteData($"<1{zone}TR{balanceStr}");
        }
    }
}
