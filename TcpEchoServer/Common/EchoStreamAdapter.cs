using Nito.AsyncEx;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Common
{
    public class EchoStreamAdapter : IDisposable
    {
        public event Action<string> OnMessageReceived;

        private readonly TcpClient _tcpClient;
        private readonly NetworkStream _stream;
        private readonly CancellationTokenSource _cancelReading;
        private readonly AsyncLock _asyncLock = new AsyncLock();

        private bool _disconnected = false;
        private const int _headerSizeInBytes = 4;

        public EchoStreamAdapter(TcpClient tcpClient)
        {
            _tcpClient = tcpClient;
            _stream = _tcpClient.GetStream();
            _cancelReading = new CancellationTokenSource();
        }

        public void StartRead()
        {
            Task.Run(async () =>
            {
                try
                {
                    while (_disconnected == false)
                    {
                        string message = await ReadMessage();
                        OnMessageReceived.Invoke(message);
                    }
                }
                catch (Exception exception)
                {
                    Logger.Error(exception.Message);
                }

            }, _cancelReading.Token);
        }

        private async Task<int> ReadIntAsync()
        {
            byte[] buffer = new byte[_headerSizeInBytes];

            int bytesRead = 0;
            int chunkSize = 1;
            while (bytesRead < buffer.Length && chunkSize > 0)
                bytesRead += chunkSize =
                    await _stream.ReadAsync(buffer, bytesRead, buffer.Length - bytesRead);

            int value = BitConverter.ToInt32(buffer);
            return value;
        }

        private async Task<string> ReadStringAsync(int messageSizeInBytes)
        {
            byte[] buffer = new byte[messageSizeInBytes];

            int bytesRead = 0;
            int chunkSize = 1;
            while (bytesRead < buffer.Length && chunkSize > 0)
                bytesRead += chunkSize =
                  await _stream.ReadAsync(buffer, bytesRead, buffer.Length - bytesRead);

            string message = Encoding.ASCII.GetString(buffer);

            return message;
        }

        public async Task WriteAsync(string message)
        {
            byte[] messageInByte = Encoding.ASCII.GetBytes(message);
            int messageSize = messageInByte.Length;
            byte[] messageSizeInByte = BitConverter.GetBytes(messageSize);

            int totalMessageLength = messageSizeInByte.Length + messageInByte.Length;

            List<byte> wholeMessage = new List<byte>(totalMessageLength);
            wholeMessage.AddRange(messageSizeInByte);
            wholeMessage.AddRange(messageInByte);
            var messageInArray = wholeMessage.ToArray();

            using (await _asyncLock.LockAsync())
                await _stream.WriteAsync(messageInArray, 0, messageInArray.Length);
        }

        private async Task<string> ReadMessage()
        {
            int messageSize = await ReadIntAsync();
            string message = await ReadStringAsync(messageSize);
            return message;
        }

        public void Dispose()
        {
            _disconnected = true;
            _cancelReading.Cancel();
            _stream.Dispose();
            _tcpClient.Close();
        }
    }
}
