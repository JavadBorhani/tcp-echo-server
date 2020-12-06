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
            await Task.Yield();
            try
            {
                int newClientId = Interlocked.Increment(ref _clientId);

                NetStreamHandler clientHandler = new NetStreamHandler(client, newClientId);
                clientHandler.OnMessageReceived += OnTcpServerMessageReceived;
                clientHandler.OnDisconnected += OnClientDisconnect;
                clientHandler.StartListenStream();

                Logger.Info("Server with id {0} connected", newClientId);
                _clients.Add(newClientId, clientHandler);
            }
            catch (Exception ex)
            {
                Logger.Error("Accepting client error: {0}", ex.Message);
            }
        }

        private void OnTcpServerMessageReceived(string message)
        {
            IEnumerable<NetStreamHandler> clients = _clients.GetAllClients();

            foreach (var client in clients)
                client.WriteAsync(message).NoAwait();
        }

        private void OnClientDisconnect(int clientId)
        {
            _clients.Remove(clientId);

            Logger.Info("Server with id {0} disconnected", clientId);
        }

        public void Stop()
        {
            try
            {
                IEnumerable<NetStreamHandler> clients = _clients.GetAllClients();

                foreach(var client in clients)
                    client.Disconnect();
                    
                _backbone?.Stop();
            }
            catch (SocketException exception)
            {
                Logger.Error(exception.Message);
            }
        }

    }
}
