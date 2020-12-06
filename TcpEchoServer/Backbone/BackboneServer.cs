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
        private readonly ConcurrentCollection<int, NetStreamHandler> _clients;

        private TcpListener _backbone;

        private int _clientId;
        private bool _disconnected;

        public BackboneServer(IPEndPoint backboneIP)
        {
            _backboneIP = backboneIP;
            _clients = new ConcurrentCollection<int, NetStreamHandler>();

            _clientId = 0;
            _disconnected = false;  
        }

        private async Task InitializeServer()
        {
            try
            {
                _backbone = new TcpListener(_backboneIP);
                _backbone.Start();

                while (_disconnected == false)
                {
                    TcpClient newClient = await _backbone.AcceptTcpClientAsync();
                    Task.Run(() => Accept(newClient)).NoAwait();
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

        private void Accept(TcpClient client)
        {
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
            IEnumerable<NetStreamHandler> clients = _clients.GetAll();

            foreach (NetStreamHandler client in clients)
                client.WriteAsync(message).NoAwait();
        }

        private void OnClientDisconnect(int clientId)
        {
            _clients.Remove(clientId);

            Logger.Info("Server with id {0} disconnected", clientId);
        }

        public void Start()
        {
            InitializeServer().NoAwait();
        }

        public void Stop()
        {
            try
            {
                _disconnected = false;
                _backbone?.Stop();

                IEnumerable<NetStreamHandler> clients = _clients.GetAll();

                foreach(var client in clients)
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
