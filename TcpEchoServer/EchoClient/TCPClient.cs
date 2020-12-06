using Common.NetStream;
using Common.Utility;
using System;
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

        private int _totalReceivedMessages;
        public int TotalReceivedMessages
        {
            get { return _totalReceivedMessages; }
        }

        private readonly IPEndPoint _currentServerAddress;
        private readonly IPEndPointProvider _ipEndPointProvider;

        public EchoClient(IPEndPointProvider ipEndPointProvider)
        {
            _clientId = 0;
            _totalReceivedMessages = 0;
            _ipEndPointProvider = ipEndPointProvider;
            _currentServerAddress = _ipEndPointProvider.GetNewAddress();
        }

        public void Dispose()
        {
            _client.Disconnect();
        }

        public void Start()
        {
            try
            {
                TcpClient tcpClient = new TcpClient();
                tcpClient.Connect(_currentServerAddress.Address, _currentServerAddress.Port);

                _client = new NetStreamHandler(tcpClient, _clientId);
                _client.OnMessageReceived += OnMessageReceived;
                _client.StartListenStream();

            }
            catch (Exception e)
            {
                Logger.Error(e.Message);
            }
        }


        public void OnMessageReceived(string message)
        {
            Interlocked.Increment(ref _totalReceivedMessages);
            Logger.Info(message);
        }

        public async Task WriteMessage(string message)
        {
            await _client.WriteAsync(message);
        }

    }
}
