namespace Alexa.NET.Skills.Insteon.Service.Models;

public enum FanSpeed : byte
{
    OFF = 0x00,
    LOW = 0x40,
    MEDIUM = 0xC0,
    HIGH = 0xFF
}