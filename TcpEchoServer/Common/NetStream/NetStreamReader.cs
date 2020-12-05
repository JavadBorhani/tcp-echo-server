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

            int bytesRead = 0;
            int chunkSize = 1;
            while (bytesRead < buffer.Length && chunkSize > 0)
                bytesRead += chunkSize =
                  await _stream.ReadAsync(buffer, bytesRead, buffer.Length - bytesRead);

            return buffer;
        }

        private async Task<int> ReadHeaderAsync()
        {
            byte[] buffer = await ReadBufferAsync(HeaderSize);

            if (buffer.Length == 0)
                return 0;

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
            if (messageSize == 0)
                return null;

            string message = await ReadMessageAsync(messageSize);
            return message;
        }

    }
}
