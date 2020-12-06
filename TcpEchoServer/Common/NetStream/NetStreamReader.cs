using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Common.NetStream
{

    internal class NetStreamReader
    {
        private readonly NetworkStream _stream;
        private const int HeaderSize = 4;

        internal NetStreamReader(NetworkStream stream)
        {
            _stream = stream;
        }

        private async Task<byte[]> ReadBufferAsync(int bufferSize)
        {
            byte[] buffer = new byte[bufferSize];

            int bytesReadCount = 0;
            int chunkSize = 1;

            while (bytesReadCount < buffer.Length && chunkSize > 0)
            {
                chunkSize = await _stream.ReadAsync(buffer, bytesReadCount, buffer.Length - bytesReadCount);
                bytesReadCount += chunkSize;
            }

            if (bytesReadCount != bufferSize)
                throw new InCompleteMessageException();

            return buffer;
        }

        private async Task<int> ReadHeaderAsync()
        {
            byte[] buffer = await ReadBufferAsync(HeaderSize);
            int messageSize = BitConverter.ToInt32(buffer);

            return messageSize;
        }

        private async Task<string> ReadMessageAsync(int messageSize)
        {
            byte[] buffer = await ReadBufferAsync(messageSize);
            string message = Encoding.ASCII.GetString(buffer);

            return message;
        }

        internal async Task<string> ReadMessage()
        {
            int messageSize = await ReadHeaderAsync();
            string message = await ReadMessageAsync(messageSize);

            return message;
        }

    }
}
