using Common.NetStream;
using Common.Utility;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Backbone
{
    public class BackboneServer
    {
        private readonly IPEndPoint _backboneIP;
        private readonly NetStreamHandlerCollection _clients;

        private TcpListener _backbone;
        private int _clientId;

        public BackboneServer(IPEndPoint backboneIP)
        {
            _backboneIP = backboneIP;
            _clients = new NetStreamHandlerCollection();
            _clientId = 0;
        }

        public void Start()
        {
            InitializeServer().NoAwait();
        }

        public async Task InitializeServer()
        {
            _backbone = new TcpListener(_backboneIP);
            try
            {
                _backbone.Start();
                while (true)
                {
                    TcpClient newClient = await _backbone.AcceptTcpClientAsync();
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
                _backbone.Stop();
            }
        }

        private async Task Accept(TcpClient client)
        {
            try
            {
                int newClientId = Interlocked.Increment(ref _clientId);

                NetStreamHandler clientHandler = new NetStreamHandler(client, newClientId);
                clientHandler.OnMessageReceived += OnTcpServerMessageReceived;
                clientHandler.OnDisconnected += OnClientDisconnect;
                clientHandler.StartListenStream();

                Logger.Info("Server with id {0} connected", newClientId);
                await _clients.Add(newClientId, clientHandler);
            }
            catch (Exception ex)
            {
                Logger.Error("Accepting client error: {0}", ex.Message);
            }
        }

        private async void OnTcpServerMessageReceived(string message)
        {
            var clients = await _clients.GetAllClients();

            for (int i = 0; i < clients.Count; ++i)
                clients[i].WriteAsync(message).NoAwait();

        }

        private async void OnClientDisconnect(int clientId)
        {
            await _clients.Remove(clientId);
            Logger.Info("Server with id {0} disconnected", clientId);
        }

        public async Task Stop()
        {
            try
            {
                List<NetStreamHandler> clients = await _clients.GetAllClients();

                for (int i = 0; i < clients.Count; ++i)
                    clients[i].Disconnect();

                _backbone?.Stop();
            }
            catch (SocketException exception)
            {
                Logger.Error(exception.Message);
            }
        }

    }
}
