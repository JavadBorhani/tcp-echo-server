using Common;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EchoServer
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

        private async Task WriteAsync(string message)
        {
            byte[] messageInByte = Encoding.ASCII.GetBytes(message);
            int messageSize = messageInByte.Length;
            byte[] messageSizeInByte = BitConverter.GetBytes(messageSize);

            int totalMessageLength = messageSizeInByte.Length + messageInByte.Length;

            List<byte> wholeMessage = new List<byte>(totalMessageLength);
            wholeMessage.AddRange(messageSizeInByte);
            wholeMessage.AddRange(messageInByte);
            var messageInArray = wholeMessage.ToArray();

            await _stream.WriteAsync(messageInArray , 0 , messageInArray.Length);
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

    public class TCPServer
    {
        private readonly IPEndPoint _serverIP;
        private readonly IPEndPoint _backboneIP;
        private readonly ConcurrentDictionary<Guid, ClientHandler> _clients = new ConcurrentDictionary<Guid, ClientHandler>();

        private TcpListener _listener;
        private TcpClient _backboneClient;

        public TCPServer(IPEndPoint serverIP, IPEndPoint backboneIP)
        {
            _serverIP = serverIP;
            _backboneIP = backboneIP;
        }

        public async Task StartAsync()
        {
            _listener = new TcpListener(_serverIP);
            _listener.Start();
            try
            {
                while (true)
                {
                    Accept(await _listener.AcceptTcpClientAsync());
                }
            }
            finally
            {
                _listener.Stop();
            }
            //todo : connect to backbone here 
        }

        private async Task Accept(TcpClient client)
        {
            ClientHandler clientHandler = new ClientHandler(client);
            clientHandler.OnMessageReceived += (message) => Logger.Info(message);
            _clients.TryAdd(Guid.NewGuid(), clientHandler);
            clientHandler.StartRead();
        }

        public void Stop()
        {
            try
            {
                foreach (var client in _clients.Values)
                    client.Dispose();

                _listener.Stop();
            }
            catch (SocketException exception)
            {
                Logger.Error(exception.Message);
            }
        }

    }
}
