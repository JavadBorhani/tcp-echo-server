using Common.Utility;
using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Common.NetStream
{
    public class NetStreamHandler : IDisposable
    {
        public event Action<string> OnMessageReceived;
        public event Action<int> OnDisconnected;

        private readonly TcpClient _tcpClient;
        private readonly NetStreamReader _streamReader;
        private readonly NetStreamWriter _streamWriter;

        private bool _disconnected = false;
        private readonly CancellationTokenSource _cancelSource;
        private readonly int _clientId; 

        public NetStreamHandler(TcpClient tcpClient, int clientId)
        {
            _tcpClient = tcpClient;
            _clientId = clientId;
            NetworkStream stream = _tcpClient.GetStream();

            _cancelSource = new CancellationTokenSource();
            _streamReader = new NetStreamReader(stream);
            _streamWriter = new NetStreamWriter(stream);
        }

        private async Task ReadingFromStreamAsync(CancellationToken cancellationToken)
        {
            try
            {
                while (_disconnected == false)
                {
                    string message = await _streamReader.ReadMessage();
                    OnMessageReceived?.Invoke(message);

                    cancellationToken.ThrowIfCancellationRequested();
                }
            }
            catch (OperationCanceledException ex)
            {
                Logger.Error("operation canceled {0}", ex.Message);
            }
            catch (InCompleteMessageException ex)
            {
                Logger.Error("incomplete message {0}" , ex.Message);
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
            }
            finally
            {
                Disconnect();
            }
        }

        public void Disconnect()
        {
            if (_disconnected == false)
            {
                _disconnected = true;
                _cancelSource.Cancel();
                _tcpClient.Close();
                OnDisconnected?.Invoke(_clientId);
            }
        }

        public void StartListenStream()
        {
            Task.Run(async () => await ReadingFromStreamAsync(_cancelSource.Token));
        }

        public async Task WriteAsync(string message)
        {
            try
            {
                await _streamWriter.WriteAsync(message, _cancelSource.Token);
            }
            catch (Exception exception)
            {
                Logger.Error(exception.Message);
                Disconnect();
            }
        }

        void IDisposable.Dispose()
        {
            Disconnect();
        }
    }
}
