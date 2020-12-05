﻿using Common.Utility;
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
        private readonly CancellationTokenSource _streamSource;
        private readonly int _clientId; 

        public NetStreamHandler(TcpClient tcpClient, int clientId)
        {
            _tcpClient = tcpClient;
            _clientId = clientId;
            var stream = _tcpClient.GetStream();
            _streamSource = new CancellationTokenSource();
            _streamReader = new NetStreamReader(stream);
            _streamWriter = new NetStreamWriter(stream);
        }

        private async Task ReadingFromStreamAsync()
        {
            try
            {
                while (_disconnected == false)
                {
                    string message = await _streamReader.ReadMessage();

                    if (message == null)
                        break;  

                    OnMessageReceived?.Invoke(message);
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception.Message);
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
                _streamSource.Cancel();
                _tcpClient.Close();
                OnDisconnected.Invoke(_clientId);
            }
        }

        public void StartReadingStream()
        {
            Task.Run(async () => await ReadingFromStreamAsync(), _streamSource.Token);
        }

        public async Task WriteAsync(string message)
        {
            try
            {
                await _streamWriter.WriteAsync(message, _streamSource.Token);
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