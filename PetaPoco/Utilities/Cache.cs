using System;
using System.Collections.Concurrent;

namespace PetaPoco.Internal
{
	class Cache<TKey, TValue>
	{
		ConcurrentDictionary<TKey, Lazy<TValue>> _map = new ConcurrentDictionary<TKey, Lazy<TValue>>();

		public int Count
		{
			get
			{
				return _map.Count;
			}
		}

		public TValue Get(TKey key, Func<TValue> factory)
		{
			return _map.GetOrAdd(key, new Lazy<TValue>(factory)).Value;
		}

		public void Flush()
		{
			_map.Clear();
		}
	}
}
