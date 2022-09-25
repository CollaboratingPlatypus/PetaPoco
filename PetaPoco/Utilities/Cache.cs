using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace PetaPoco.Internal
{
    internal class Cache<TKey, TValue>
    {
        private readonly ConcurrentDictionary<TKey, Lazy<TValue>> _map = new ConcurrentDictionary<TKey, Lazy<TValue>>();

        public int Count => _map.Count;

        public TValue GetOrAdd(TKey key, Func<TValue> factory)
        {
            var cachedItem = _map.GetOrAdd(key, new Lazy<TValue>(factory));
            return cachedItem.Value;
        }

        public void Flush() => _map.Clear();
    }
}