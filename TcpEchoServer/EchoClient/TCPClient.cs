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
        private EchoStreamAdapter _client;

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
                _client = new EchoStreamAdapter(_tcpClient);
                _client.StartRead();
            }
            catch(Exception e)
            {
                Logger.Error(e.Message);
            }
        }

        public async Task WriteMessage(string message)
        {
            await _client.WriteAsync(message);
        }

    }
}
