using Common.NetStream;
using Common.Utility;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace EchoClient
{

    public class EchoClient : IDisposable
    {
        private NetStreamHandler _client;
        private int _clientId;
        private int _retry = 0;
        private int _maxRetry = 3;
        private bool _disconnected = false;

        private IPEndPoint _currentServerAddress;
        private readonly IPEndPointProvider _ipEndPointProvider;
        private readonly ClientStats _clientStats;

        public EchoClient(int clientId, List<IPEndPoint> endPoints, ClientStats clientStats)
        {
            _clientId = clientId;
            _ipEndPointProvider = new IPEndPointProvider(endPoints);
            _clientStats = clientStats;
        }

        private async Task Connect()
        {
            try
            {
                TcpClient tcpClient = new TcpClient();
                await tcpClient.ConnectAsync(_currentServerAddress.Address, _currentServerAddress.Port);

                _client = new NetStreamHandler(tcpClient, _clientId);
                _client.OnMessageReceived += OnMessageReceived;
                _client.OnDisconnected += OnDisconnected;
                _client.StartListenStream();

                Logger.Info("connect to {0}", _currentServerAddress);
            }
            catch (Exception e)
            {
                Logger.Error(e.Message);
            }
        }

        private async void OnDisconnected(int clientId)
        {
            Logger.Error("disconnected from {0}", _currentServerAddress);

            await Task.Delay(1000);
            _client.OnMessageReceived -= OnMessageReceived;
            _client.OnDisconnected -= OnDisconnected;

            if (_retry == _maxRetry)
            {
                _retry = 0;
                _currentServerAddress = _ipEndPointProvider.GetNewAddress();
            }

            _retry++;
            await Connect();
        }

        public void OnMessageReceived(string message)
        {
            Interlocked.Increment(ref _clientStats.TotalMessageRecieved);
            Logger.Info(message);
        }

        public async Task Start()
        {
            _currentServerAddress = _ipEndPointProvider.GetNewAddress();
            await Connect();
        }

        public async Task WriteMessage(string message)
        {
            Interlocked.Increment(ref _clientStats.TotalMessageSent);
            await _client.WriteAsync(message);
        }

        public void Stop()
        {
            if (_disconnected == false)
            {
                _disconnected = true;
                _client.OnDisconnected -= OnDisconnected;
                _client.OnMessageReceived -= OnMessageReceived;
                _client.Disconnect();
            }
        }

        void IDisposable.Dispose()
        {
            Stop();
        }

    }
}
