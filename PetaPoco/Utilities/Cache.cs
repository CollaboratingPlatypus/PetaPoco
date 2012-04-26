using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace PetaPoco
{
	class Cache<TKey, TValue>
	{
		Dictionary<TKey, TValue> _map = new Dictionary<TKey, TValue>();
		ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();

		public int Count
		{
			get
			{
				return _map.Count;
			}
		}

		public TValue Get(TKey key, Func<TValue> factory)
		{
			// Check cache
			_lock.EnterReadLock();
			TValue val;
			try
			{
				if (_map.TryGetValue(key, out val))
					return val;
			}
			finally
			{
				_lock.ExitReadLock();
			}


			// Cache it
			_lock.EnterWriteLock();
			try
			{
				// Check again
				if (_map.TryGetValue(key, out val))
					return val;

				// Create it
				val = factory();

				// Store it
				_map.Add(key, val);

				// Done
				return val;
			}
			finally
			{
				_lock.ExitWriteLock();
			}
		}

		public void Flush()
		{
			// Cache it
			_lock.EnterWriteLock();
			try
			{
				_map.Clear();
			}
			finally
			{
				_lock.ExitWriteLock();
			}

		}
	}
}
