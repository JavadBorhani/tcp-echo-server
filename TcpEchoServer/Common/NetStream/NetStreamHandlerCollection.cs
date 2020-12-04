using Nito.AsyncEx;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Common.NetStream
{
    public class NetStreamHandlerCollection
    {
        private readonly Dictionary<int, NetStreamHandler> _collection;
        private readonly AsyncReaderWriterLock _asyncReaderWriterLock;

        public NetStreamHandlerCollection()
        {
            _collection = new Dictionary<int, NetStreamHandler>();
            _asyncReaderWriterLock = new AsyncReaderWriterLock();
        }

        public async Task<List<NetStreamHandler>> GetAllClients()
        {
            using (await _asyncReaderWriterLock.ReaderLockAsync())
                return _collection.Values.ToList();
        }

        public async Task Add(int id, NetStreamHandler streamHandler)
        {
            using (await _asyncReaderWriterLock.WriterLockAsync())
                _collection.Add(id, streamHandler);
        }

        public async Task Remove(int id)
        {
            using (await _asyncReaderWriterLock.WriterLockAsync())
                _collection.Remove(id, out var streamHandler);
        }

        public async Task ClearAll()
        {
            using (await _asyncReaderWriterLock.WriterLockAsync())
                _collection.Clear();
        }
    }
}
