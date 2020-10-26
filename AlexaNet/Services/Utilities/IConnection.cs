using System;

namespace AlexaNet.Services.Utilities
{
    public interface IConnection : IDisposable
    {
        string OpenConnection();
        string WriteData(string data, int expectedLinesOfResponse = 0);
    }
}
