using System;
using System.Collections.Generic;
using System.Linq;
using Alexa.NET.SmartHome.Utilities;

namespace Alexa.NET.Skills.Monoprice.Service
{
    public class MonopriceService : IDisposable
    {
        private readonly TcpPortConnection[] _conns;

        public MonopriceService(string ipAddress, params int[] tcpPorts)
        {
            _conns = new TcpPortConnection[tcpPorts.Length];
            for (var i = 0; i < tcpPorts.Length; i++)
            {
                _conns[i] = new TcpPortConnection(ipAddress, tcpPorts[i]);
                _conns[i].OpenConnection();
            }
        }

        public virtual void Dispose()
        {
            foreach (var tcpPortConnection in _conns)
                tcpPortConnection.Dispose();

        }

        public List<ZoneStatus> GetStatus()
        {
            var result = new List<ZoneStatus>();

            foreach (var conn in _conns)
            {
                var res = conn.WriteData("?10", 6);
                var lines = res.Split(new[] { "\r\n" }, StringSplitOptions.None);
                result.AddRange(lines.Skip(1).Take(6).Select(line => new ZoneStatus(line)));
            }

            //TODO: Clean this up later, but some quick-and-dirty to clean-up zone names...
            for (var i = 0; i < result.Count; i++)
                result[i].Name = $"Zone{i + 1}";

            return result;
        }

        public void SetPowerOn(string zone)
        {
            SetPowerOn(ParseZone(zone));
        }

        public void SetPowerOn(params int[] zones)
        {
            foreach (var zone in zones)
                _conns[GetControllerByZone(zone)].WriteData($"<1{GetZoneForController(zone)}PR01");
        }

        public void SetPowerOff(string zone)
        {
            SetPowerOff(ParseZone(zone));
        }

        public void SetPowerOff(params int[] zones)
        {
            foreach (var zone in zones)
                _conns[GetControllerByZone(zone)].WriteData($"<1{GetZoneForController(zone)}PR00");
        }

        public void SetMute(bool mute, string zone)
        {
            SetMute(mute, ParseZone(zone));
        }

        public void SetMute(bool mute, params int[] zones)
        {
            foreach (var zone in zones)
                _conns[GetControllerByZone(zone)].WriteData($"<1{GetZoneForController(zone)}MU0" + (mute ? "1" : "0"));
        }

        public void SetVolume(int volume, string zone)
        {
            SetVolume(volume, ParseZone(zone));
        }

        public void SetVolume(int volume, params int[] zones)
        {
            if (volume < 0 || volume > 38)
                throw new ArgumentOutOfRangeException(nameof(volume), "The volume must be between 0 and 38.");

            var volumeStr = volume.ToString("D2");
            foreach (var zone in zones)
                _conns[GetControllerByZone(zone)].WriteData($"<1{GetZoneForController(zone)}VO{volumeStr}");
        }

        public void SetSource(int source, string zone)
        {
            SetSource(source, ParseZone(zone));
        }

        public void SetSource(int source, params int[] zones)
        {
            if (source < 1 || source > 6)
                throw new ArgumentOutOfRangeException(nameof(source), "The source must be between 1 and 6.");

            var sourceStr = source.ToString("D2");
            foreach (var zone in zones)
                _conns[GetControllerByZone(zone)].WriteData($"<1{GetZoneForController(zone)}CH{sourceStr}");
        }

        public void SetBalance(int balance, string zone)
        {
            SetBalance(balance, ParseZone(zone));
        }

        public void SetBalance(int balance, params int[] zones)
        {
            if(balance < 0 || balance > 20)
                throw new ArgumentOutOfRangeException(nameof(balance), "The balance must be between 0 and 20. (0-9 is Left, 10 is Center, and 11-20 is Right)");

            var balanceStr = balance.ToString("D2");
            foreach (var zone in zones)
                _conns[GetControllerByZone(zone)].WriteData($"<1{GetZoneForController(zone)}BL{balanceStr}");
        }

        public void SetBass(int bass, string zone)
        {
            SetBass(bass, ParseZone(zone));
        }

        public void SetBass(int bass, params int[] zones)
        {
            if (bass < 0 || bass > 14)
                throw new ArgumentOutOfRangeException(nameof(bass), "The bass must be between 0 and 14. (0-6 is Decrease, 7 is Flat, and 8-14 is Increase)");

            var balanceStr = bass.ToString("D2");
            foreach (var zone in zones)
                _conns[GetControllerByZone(zone)].WriteData($"<1{GetZoneForController(zone)}BS{balanceStr}");
        }

        public void SetTreble(int treble, string zone)
        {
            SetTreble(treble, ParseZone(zone));
        }

        public void SetTreble(int treble, params int[] zones)
        {
            if (treble < 0 || treble > 14)
                throw new ArgumentOutOfRangeException(nameof(treble), "The treble must be between 0 and 14. (0-6 is Decrease, 7 is Flat, and 8-14 is Increase)");

            var balanceStr = treble.ToString("D2");
            foreach (var zone in zones)
                _conns[GetControllerByZone(zone)].WriteData($"<1{GetZoneForController(zone)}TR{balanceStr}");
        }

        private int ParseZone(string zone)
        {
            return int.Parse(zone.Substring(4));
        }

        private int GetControllerByZone(int zone)
        {
            return (zone - 1) / 6;
        }

        private int GetZoneForController(int zone)
        {
            return ((zone - 1) % 6) + 1;
        }
    }
}
