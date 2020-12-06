using Nito.AsyncEx;
using System.Collections.Generic;
using System.Linq;

namespace Common.NetStream
{
    public class NetStreamHandlerCollection
    {
        private readonly Dictionary<int, NetStreamHandler> _collection;
        private readonly AsyncReaderWriterLock _asyncReaderWriterLock;

        private List<NetStreamHandler> _collectionValuesCache; 

        public NetStreamHandlerCollection()
        {
            _collection = new Dictionary<int, NetStreamHandler>();
            _asyncReaderWriterLock = new AsyncReaderWriterLock();
        }

        public IEnumerable<NetStreamHandler> GetAllClients()
        {
            using (_asyncReaderWriterLock.ReaderLock())
            {
                if (_collectionValuesCache == null)
                    _collectionValuesCache = _collection.Values.ToList();

                return _collectionValuesCache;
            }

        }

        public void Add(int id, NetStreamHandler streamHandler)
        {
            using (_asyncReaderWriterLock.WriterLock())
            {
                _collection.TryAdd(id, streamHandler);
                _collectionValuesCache = null;
            }
        }

        public NetStreamHandler Remove(int id)
        {
            using (_asyncReaderWriterLock.WriterLock())
            {
                _collection.Remove(id, out var streamHandler);
                _collectionValuesCache = null;

                return streamHandler;   
            }
        }

        public void ClearAll()
        {
            using (_asyncReaderWriterLock.WriterLock())
            {
                _collection.Clear();
                _collectionValuesCache = null;
            }
                
        }
    }
}
