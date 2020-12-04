using Common.NetStream;
using Common.Utility;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace EchoServer
{
    public class TCPServer
    {
        private readonly IPEndPoint _serverIP;
        private readonly IPEndPoint _backboneIP;
        private readonly NetStreamHandlerCollection _clients;

        private TcpListener _server;
        private NetStreamHandler _backbone;
        private int _clientId;

        public TCPServer(IPEndPoint serverIP, IPEndPoint backboneIP)
        {
            _serverIP = serverIP;
            _backboneIP = backboneIP;
            _clients = new NetStreamHandlerCollection();
            _clientId = 0;
        }

        public async Task StartAsync()
        {
            await ConnectToBackbone();
            StartServer().NoAwait();
        }

        public async Task ConnectToBackbone()
        {
            TcpClient client = new TcpClient();
            try
            {
                await client.ConnectAsync(_backboneIP.Address, _backboneIP.Port);

                _backbone = new NetStreamHandler(client, 0);
                _backbone.OnMessageReceived += OnBackboneMessageReceived;
                _backbone.StartReadingStream();

            }
            catch (SocketException socketException)
            {
                Logger.Error(socketException.Message);
                client.Dispose();
            }
            catch (Exception exception)
            {
                Logger.Error(exception.Message);
                client.Dispose();
            }
        }

        public async Task StartServer()
        {
            _server = new TcpListener(_serverIP);
            _server.Start();
            try
            {
                while (true)
                {
                    TcpClient newClient = await _server.AcceptTcpClientAsync();
                    Accept(newClient).NoAwait();
                }
            }
            finally
            {
                _server.Stop();
            }
        }

        public async Task Stop()
        {
            try
            {
                List<NetStreamHandler> clients = await _clients.GetAllClients();

                for (int i = 0; i < clients.Count; ++i)
                    clients[i].Disconnect();

                _backbone.Disconnect();
                _server.Stop();
            }
            catch (SocketException exception)
            {
                Logger.Error(exception.Message);
            }
        }

        private async Task Accept(TcpClient client)
        {
            await Task.Yield();
            try
            {
                int newClientId = Interlocked.Increment(ref _clientId);

                NetStreamHandler clientHandler = new NetStreamHandler(client, newClientId);
                clientHandler.OnMessageReceived += OnClientMessageReceived;
                clientHandler.OnDisconnected += OnClientDisconnect;
                clientHandler.StartReadingStream();

                Logger.Info("Client with clientId {0} connected", newClientId);
                await _clients.Add(newClientId, clientHandler);
            }
            catch(Exception ex)
            {
                Logger.Error("Accepting client error: {0}", ex.Message);
            }
        }

        private async void OnBackboneMessageReceived(string message)
        {
            List<NetStreamHandler> clients = await _clients.GetAllClients();

            for(int i = 0; i < clients.Count; ++i)
                clients[i].WriteAsync(message).NoAwait();

            Logger.Info(message);
        }

        private void OnClientMessageReceived(string message)
        {
            _backbone.WriteAsync(message).NoAwait();
        }

        private async void OnClientDisconnect(int clientId)
        {
            await _clients.Remove(clientId);
            Logger.Info("Client with clientId {0} disconnected", clientId);
        }

    }
}
