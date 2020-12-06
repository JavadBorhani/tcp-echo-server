using Nito.AsyncEx;
using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Common.NetStream
{
    internal class NetStreamWriter
    {
        private readonly NetworkStream _stream;
        private readonly AsyncLock _asyncLock = new AsyncLock();

        internal NetStreamWriter(NetworkStream stream)
        {
            _stream = stream;
        }

        internal async Task WriteAsync(string message, CancellationToken cancelToken = default(CancellationToken))
        {
            byte[] messageByteArray = Encoding.ASCII.GetBytes(message);
            int headerSize = messageByteArray.Length;

            byte[] header = BitConverter.GetBytes(headerSize);
            int totalMessageLength = header.Length + messageByteArray.Length;

            byte[] totalMessage = new byte[totalMessageLength];
            Buffer.BlockCopy(header, 0, totalMessage, 0, header.Length);
            Buffer.BlockCopy(messageByteArray, 0, totalMessage, header.Length, messageByteArray.Length);

            using (await _asyncLock.LockAsync())
                await _stream.WriteAsync(totalMessage, 0, totalMessage.Length, cancelToken);
        }
    }
}
