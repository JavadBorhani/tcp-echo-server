using Nito.AsyncEx;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Common.NetStream
{
    public class NetStreamHandlerCollection
    {
        private readonly Dictionary<int, NetStreamHandler> _streams;
        private readonly AsyncReaderWriterLock _asyncReaderWriterLock;

        public NetStreamHandlerCollection()
        {
            _streams = new Dictionary<int, NetStreamHandler>();
            _asyncReaderWriterLock = new AsyncReaderWriterLock();
        }

        public async Task<List<NetStreamHandler>> GetAllClients()
        {
            using (await _asyncReaderWriterLock.ReaderLockAsync())
                return _streams.Values.ToList();
        }

        public async Task Add(int id, NetStreamHandler streamHandler)
        {
            using (await _asyncReaderWriterLock.WriterLockAsync())
                _streams.Add(id, streamHandler);
        }

        public async Task Remove(int id)
        {
            using (await _asyncReaderWriterLock.WriterLockAsync())
                _streams.Remove(id, out var streamHandler);
        }
    }
}
