namespace Alexa.NET.Skills.Insteon.Service.Models;

public enum FanSpeed : byte
{
    //These Hex values are extracted from the Insteon API documentation.
    OFF = 0x00,
    LOW = 0x55,
    MEDIUM = 0xAA,
    HIGH = 0xFF
}