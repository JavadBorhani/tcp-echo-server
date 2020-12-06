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
    public struct ServerStats
    {
        public int ClientCounts;
    }

    public class EchoServer
    {
        private readonly IPEndPoint _serverIP;
        private readonly IPEndPoint _backboneIP;
        private readonly NetStreamHandlerCollection _clients;

        public ServerStats ServerStats;
        private TcpListener _server;
        private NetStreamHandler _backbone;

        private int _clientId;
        private int _backboneId;
        private bool _disconnected;

        public EchoServer(IPEndPoint serverIP, IPEndPoint backboneIP)
        {
            _serverIP = serverIP;
            _backboneIP = backboneIP;
            _clients = new NetStreamHandlerCollection();
            ServerStats = new ServerStats();

            _clientId = 0;
            _backboneId = -1;
            _disconnected = false;
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
                clientHandler.StartListenStream();

                Logger.Info("Client with clientId {0} connected", newClientId);

                _clients.Add(newClientId, clientHandler);

                Interlocked.Increment(ref ServerStats.ClientCounts);

            }
            catch (Exception ex)
            {
                Logger.Error("Accepting client error: {0}", ex.Message);
            }
        }

        private async Task DisconnectClientRandomly(int delayInSeconds)
        {
            await Task.Delay(delayInSeconds * 1000);
            int randomClientId = _clients.GetRandomClientId();

            Logger.Info("diconnect client {0} after 3 seconds", randomClientId);
            DisconnectClientAfterSeconds(randomClientId, 3).NoAwait();
        }

        private async Task DisconnectClientAfterSeconds(int clientId , int seconds)
        {
            await Task.Delay(seconds * 1000);
            NetStreamHandler client = _clients.Remove(clientId);
            client?.Disconnect();
        }

        private void OnBackboneMessageReceived(string message)
        {
            IEnumerable<NetStreamHandler> clients = _clients.GetAll();

            foreach (var client in clients)
                client.WriteAsync(message).NoAwait();

            Logger.Info(message);
        }

        private void OnDisconnectedFromBackbone(int id)
        {
            Logger.Error("Disconnected from backbone {0}", id);
        }

        private void OnClientMessageReceived(string message)
        {
            _backbone.WriteAsync(message).NoAwait();
        }

        private void OnClientDisconnect(int clientId)
        {
            _clients.Remove(clientId);

            Interlocked.Decrement(ref ServerStats.ClientCounts);
            Logger.Info("Client with clientId {0} disconnected", clientId);
        }

        private void ConnectToBackbone()
        {
            try
            {
                TcpClient client = new TcpClient();
                client.Connect(_backboneIP.Address, _backboneIP.Port);

                _backbone = new NetStreamHandler(client, _backboneId);
                _backbone.OnMessageReceived += OnBackboneMessageReceived;
                _backbone.OnDisconnected += OnDisconnectedFromBackbone;
                _backbone.StartListenStream();

            }
            catch (SocketException socketException)
            {
                Logger.Error("could not connect to backbone!!!");
                Logger.Error(socketException.Message);

            }
            catch (Exception exception)
            {
                Logger.Error(exception.Message);

            }
        }

        private async Task InitializeServer()
        {
            try
            {
                _server = new TcpListener(_serverIP);
                _server.Start();

                while (_disconnected == false)
                {
                    TcpClient newClient = await _server.AcceptTcpClientAsync();
                    Accept(newClient).NoAwait();
                }
            }
            catch (SocketException socketException)
            {
                Logger.Error("Socket exception {0}", socketException.Message);
            }
            catch (Exception exception)
            {
                Logger.Error("exception {0}", exception.Message);
            }
            finally
            {
                Stop();
            }
        }

        public void Start()
        {
            ConnectToBackbone();
            InitializeServer().NoAwait();
            //DisconnectClientRandomly(delayInSeconds: 10).NoAwait();
        }

        public void Stop()
        {
            try
            {
                _disconnected = false;
                _backbone?.Disconnect();
                _server?.Stop();

                IEnumerable<NetStreamHandler> clients = _clients.GetAll();
                foreach (var client in clients)
                    client.Disconnect();

                _clients.ClearAll();
            }
            catch (SocketException exception)
            {
                Logger.Error(exception.Message);
            }
        }

    }
}
