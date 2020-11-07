using System;

namespace AlexaNet.Infrastructure.Services.Utilities
{
    public interface IConnection : IDisposable
    {
        string OpenConnection();
        string WriteData(string data, int expectedLinesOfResponse = 0);
    }
}
