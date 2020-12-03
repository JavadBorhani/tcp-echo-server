using Nito.AsyncEx;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class NetStreamWriter
    {
        private readonly NetworkStream _stream;
        private readonly AsyncLock _asyncLock = new AsyncLock();

        public NetStreamWriter(NetworkStream stream)
        {
            _stream = stream;
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
    }
}
