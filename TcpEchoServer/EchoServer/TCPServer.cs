using Common;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EchoServer
{
    public class TCPServer
    {
        private readonly IPEndPoint _serverIP;
        private readonly IPEndPoint _backboneIP;
        private readonly ConcurrentDictionary<Guid, EchoStreamAdapter> _clients = new ConcurrentDictionary<Guid, EchoStreamAdapter>();

        private TcpListener _listener;
        private TcpClient _backboneClient;

        public TCPServer(IPEndPoint serverIP, IPEndPoint backboneIP)
        {
            _serverIP = serverIP;
            _backboneIP = backboneIP;
        }

        public async Task StartAsync()
        {
            _listener = new TcpListener(_serverIP);
            _listener.Start();
            try
            {
                while (true)
                {
                    Accept(await _listener.AcceptTcpClientAsync());
                }
            }
            finally
            {
                _listener.Stop();
            }
            //todo : connect to backbone here 
        }

        private async Task Accept(TcpClient client)
        {
            EchoStreamAdapter clientHandler = new EchoStreamAdapter(client);
            clientHandler.OnMessageReceived += (message) => Logger.Info(message);
            _clients.TryAdd(Guid.NewGuid(), clientHandler);
            clientHandler.StartRead();
        }

        public void Stop()
        {
            try
            {
                foreach (var client in _clients.Values)
                    client.Dispose();

                _listener.Stop();
            }
            catch (SocketException exception)
            {
                Logger.Error(exception.Message);
            }
        }

    }
}
