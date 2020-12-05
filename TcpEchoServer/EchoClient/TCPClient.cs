using Common.NetStream;
using Common.Utility;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace EchoClient
{
    public class TCPClient : IDisposable
    {
        private readonly IPEndPoint _destinationIP;
        private TcpClient _tcpClient;
        private NetStreamHandler _client;
        private int _clientId;
        private int _totalReceivedMessages;

        public int TotalReceivedMessages
        {
            get { return _totalReceivedMessages; }
        }

        public TCPClient(IPEndPoint ip)
        {
            _destinationIP = ip;
            _clientId = 0;
            _totalReceivedMessages = 0;
        }

        public void Dispose()
        {
            _client.Disconnect();
            _tcpClient.Close();
        }

        public async Task StartAsync()
        {
            try
            {
                _tcpClient = new TcpClient();
                _tcpClient.Connect(_destinationIP.Address, _destinationIP.Port);

                _client = new NetStreamHandler(_tcpClient, _clientId);
                _client.OnMessageReceived += OnMessageReceived;
                _client.StartListenStream();

            }
            catch (Exception e)
            {
                _tcpClient.Close();
                Logger.Error(e.Message);
            }
        }


        public void OnMessageReceived(string message)
        {
            Interlocked.Increment(ref _totalReceivedMessages);
            Logger.Info(message);
        }

        public async Task WriteMessage(string message)
        {
            await _client.WriteAsync(message);
        }

    }
}
