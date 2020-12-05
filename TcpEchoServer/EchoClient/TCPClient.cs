using Common.NetStream;
using Common.Utility;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace EchoClient
{
    public class TCPClient : IDisposable
    {
        private readonly IPEndPoint _destinationIP;
        private TcpClient _tcpClient;
        private NetStreamHandler _client;
        private int _clientId = 0; 

        public TCPClient(IPEndPoint ip)
        {
            _destinationIP = ip;
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
                await _tcpClient.ConnectAsync(_destinationIP.Address, _destinationIP.Port);

                _client = new NetStreamHandler(_tcpClient , _clientId);
                _client.StartListenStream();
                _client.OnMessageReceived += OnMessageReceived;
            }
            catch(Exception e)
            {
                _tcpClient.Close();
                Logger.Error(e.Message);
            }
        }

        public void OnMessageReceived(string message)
        {
            Logger.Info(message);
        }

        public async Task WriteMessage(string message)
        {
            await _client.WriteAsync(message);
        }

    }
}
