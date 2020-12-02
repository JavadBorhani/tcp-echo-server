using Common;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EchoClient
{
    public class ClientHandler : IDisposable
    {
        public event Action<string> OnMessageReceived;

        private readonly TcpClient _tcpClient;
        private readonly NetworkStream _stream;
        private readonly CancellationTokenSource _cancelReading;

        private bool _disconnected = false;
        private const int _headerSizeInBytes = 4;

        public ClientHandler(TcpClient tcpClient)
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

            string message = BitConverter.ToString(buffer);

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



    public class TCPClient : IDisposable
    {
        private readonly IPEndPoint _destinationIP;

        private TcpClient _tcpClient;
        private ClientHandler _client;

        public TCPClient(IPEndPoint ip)
        {
            _destinationIP = ip;
        }

        public void Dispose()
        {
            _client.Dispose();
            _tcpClient.Close();
        }

        public async Task StartAsync()
        {
            try
            {
                _tcpClient = new TcpClient();
                await _tcpClient.ConnectAsync(_destinationIP.Address, _destinationIP.Port);
                _client = new ClientHandler(_tcpClient);
                _client.StartRead();
            }
            catch(Exception e)
            {
                Logger.Error(e.Message);
            }
        }

        public async Task WriteMessage(string message)
        {
            await _client.WriteAsync(message);
        }

    }
}
