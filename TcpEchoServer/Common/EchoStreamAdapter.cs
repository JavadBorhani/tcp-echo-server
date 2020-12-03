using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Common
{
    public class NetStreamAdapter : IDisposable
    {
        public event Action<string> OnMessageReceived;
        public event Action<int> OnDisconnected;

        private readonly TcpClient _tcpClient;
        private readonly int _clientId;
        private readonly NetworkStream _stream;
        private readonly CancellationTokenSource _cancelReading;

        private bool _disconnected = false;
        private const int _headerSizeInBytes = 4;

        private readonly NetStreamReader _streamReader;
        private readonly NetStreamWriter _streamWriter;

        public NetStreamAdapter(TcpClient tcpClient, int clientId)
        {
            _tcpClient = tcpClient;
            _clientId = clientId;
            _stream = _tcpClient.GetStream();
            _cancelReading = new CancellationTokenSource();
            _streamReader = new NetStreamReader(_stream, _headerSizeInBytes);
            _streamWriter = new NetStreamWriter(_stream);
        }

        public void StartRead()
        {
            Task.Run(async () =>
            {
                try
                {
                    while (_disconnected == false)
                    {
                        string message = await _streamReader.ReadMessage();
                        OnMessageReceived?.Invoke(message);
                    }
                }
                catch (Exception exception)
                {
                    Logger.Error(exception.Message);
                }

            }, _cancelReading.Token);
        }


        public async Task WriteAsync(string message)
        {
            try
            {
                await _streamWriter.WriteAsync(message);
            }
            catch (Exception exception)
            {
                Logger.Error(exception.Message);
                Disconnect();
            }

        }

        private void Disconnect()
        {
            if (_disconnected == false)
            {
                _disconnected = true;
                _cancelReading.Cancel();
                _stream.Dispose();
                _tcpClient.Close();
                OnDisconnected.Invoke(_clientId);
            }
        }

        public void Dispose()
        {
            Disconnect();
        }
    }
}
