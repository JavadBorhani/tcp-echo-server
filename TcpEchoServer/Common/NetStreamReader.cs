using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class NetStreamReader
    {
        private readonly NetworkStream _stream;
        private readonly int _headerSize;

        public NetStreamReader(NetworkStream stream, int headerSize)
        {
            _stream = stream;
            _headerSize = headerSize;
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
            byte[] buffer = await ReadBufferAsync(_headerSize);
            int messageSize = BitConverter.ToInt32(buffer);

            return messageSize;
        }

        private async Task<string> ReadMessageAsync(int messageSize)
        {
            byte[] buffer = await ReadBufferAsync(messageSize);
            string message = Encoding.ASCII.GetString(buffer);

            return message;
        }

        public async Task<string> ReadMessage()
        {
            int messageSize = await ReadHeaderAsync();
            string message = await ReadMessageAsync(messageSize);
            return message;
        }

    }
}
