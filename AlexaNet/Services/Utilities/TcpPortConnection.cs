using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;

namespace AlexaNet.Services.Utilities
{
    public class TcpPortConnection : IConnection
    {
        private readonly string _ipAddress;
        private readonly int _port;
        private readonly Socket _clientSocket;
        private readonly object _lock = new object();

        public TcpPortConnection(string ipAddress, int port)
        {
            _ipAddress = ipAddress;
            _port = port;
            _clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp) {ReceiveTimeout = 500};
        }

        public string OpenConnection()
        {
            lock (_lock)
            {
                if (!_clientSocket.Connected)
                    _clientSocket.Connect(IPAddress.Parse(_ipAddress), _port);
            }

            return null;
        }

        public string WriteData(string data, int expectedLinesOfResponse = 0)
        {
            var match = new Regex("\\r\\n");
            var sb = new StringBuilder();
            lock (_lock)
            {
                _clientSocket.Send(Encoding.UTF8.GetBytes(data + "\r"));
                try
                {
                    var receiveBuffer = new byte[1024];
                    int receivedBytes;
                    while ((receivedBytes = _clientSocket.Receive(receiveBuffer)) != 0)
                    {
                        var s = Encoding.ASCII.GetString(receiveBuffer, 0, receivedBytes);
                        sb.Append(s);

                        var matches = match.Matches(sb.ToString());
                        if (matches.Count == expectedLinesOfResponse + 1)
                            break;
                    }
                }
                catch (SocketException)
                {
                    //Timeout reached
                }
            }

            //Ditch the # deliminator
            var response = sb.ToString().Replace("#", "");  // at this point you should have all the data that
                                                            // has been received from the remote, so far.

            return response;
        }

        public void Dispose()
        {
            //_clientSocket.Disconnect(true);
            //_clientSocket.Close();
            _clientSocket.Dispose();
        }
    }
}
