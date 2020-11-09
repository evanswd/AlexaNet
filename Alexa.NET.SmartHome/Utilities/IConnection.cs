using System;

namespace Alexa.NET.SmartHome.Utilities
{
    public interface IConnection : IDisposable
    {
        string OpenConnection();
        string WriteData(string data, int expectedLinesOfResponse = 0);
    }
}
