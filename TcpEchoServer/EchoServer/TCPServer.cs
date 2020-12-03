using Common;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        private readonly ConcurrentDictionary<int, NetStreamAdapter> _clients = new ConcurrentDictionary<int, NetStreamAdapter>();

        private TcpListener _listener;
        private TcpClient _backboneClient;
        private int _clientId = 0;

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
            await Task.Yield();
            var newClientId = Interlocked.Increment(ref _clientId);
            NetStreamAdapter clientHandler = new NetStreamAdapter(client, newClientId);
            clientHandler.OnMessageReceived += OnMessageReceived;
            clientHandler.OnDisconnected += OnClientDisconnect;
            _clients.TryAdd(newClientId, clientHandler);
            clientHandler.StartRead();
            Logger.Info("Client with clientId {0} connected", newClientId);
        }

        private void OnMessageReceived(string message)
        {
            foreach (var client in _clients.Values)
                client.WriteAsync(message);

            Logger.Info(message);
        }

        private void OnClientDisconnect(int clientId)
        {
            _clients.TryRemove(clientId, out var streamAdapter);
            Logger.Info("Client with clientId {0} disconnected", clientId);
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
