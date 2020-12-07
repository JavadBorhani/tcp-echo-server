using Nito.AsyncEx;
using System.Collections.Generic;
using System.Linq;

namespace Common.NetStream
{
    public class ConcurrentCollection<TKey, TValue> 
    {
        private readonly Dictionary<TKey, TValue> _collection;
        private readonly AsyncReaderWriterLock _asyncReaderWriterLock;

        public ConcurrentCollection()
        {
            _collection = new Dictionary<TKey, TValue>();
            _asyncReaderWriterLock = new AsyncReaderWriterLock();
        }

        public IEnumerable<TValue> GetAll()
        {
            using (_asyncReaderWriterLock.ReaderLock())
                return _collection.Values;
        }

        public List<TKey> GetAllKeys()
        {
            using (_asyncReaderWriterLock.ReaderLock())
                return _collection.Keys.ToList();
        }
        
        public int Count()
        {
            using (_asyncReaderWriterLock.ReaderLock())
                return _collection.Count;
        }

        public void Add(TKey key, TValue value)
        {
            using (_asyncReaderWriterLock.WriterLock())
                _collection.TryAdd(key, value);
        }

        public TValue Remove(TKey key)
        {
            using (_asyncReaderWriterLock.WriterLock())
            {
                _collection.Remove(key, out TValue value);
                return value;   
            }
        }

      
        public void ClearAll()
        {
            using (_asyncReaderWriterLock.WriterLock())
                _collection.Clear();    
        }
    }
}
