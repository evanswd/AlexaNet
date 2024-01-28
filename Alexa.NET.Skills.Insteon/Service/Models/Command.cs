namespace Alexa.NET.Skills.Insteon.Service.Models;

public enum Command : byte
{
    ID_REQUEST = 0x10,
    ON = 0x11,
    FAST_ON = 0x12,
    OFF = 0x13,
    FAST_OFF = 0x14,
    BRIGHTEN = 0x15,
    DIM = 0x16,
    START_MANUAL_CHANGE = 0x17,
    STOP_MANUAL_CHANGE = 0x18,
    STATUS_REQUEST = 0x19
}