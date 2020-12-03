using Common;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EchoClient
{
    public class TCPClient : IDisposable
    {
        private readonly IPEndPoint _destinationIP;
        private TcpClient _tcpClient;
        private NetStreamAdapter _client;
        private int _clientId = 0; 

        public TCPClient(IPEndPoint ip)
        {
            _destinationIP = ip;
        }

        public void Dispose()
        {
            _client.Dispose();
            _tcpClient.Close();
        }

        public async Task StartAsync()
        {
            try
            {
                _tcpClient = new TcpClient();
                await _tcpClient.ConnectAsync(_destinationIP.Address, _destinationIP.Port);
                _client = new NetStreamAdapter(_tcpClient , _clientId);
                _client.StartRead();
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
