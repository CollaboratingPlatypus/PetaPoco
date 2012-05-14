using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Threading;
using PetaPoco.Internal;

namespace PetaPoco
{
	public static class Mappers
	{
		public static void Register(Assembly assembly, IMapper mapper)
		{
			RegisterInternal(assembly, mapper);
		}

		public static void Register(Type type, IMapper mapper)
		{
			RegisterInternal(type, mapper);
		}

		public static void Revoke(Assembly assembly)
		{
			RevokeInternal(assembly);
		}

		public static void Revoke(Type type)
		{
			RevokeInternal(type);
		}

		public static void Revoke(IMapper mapper)
		{
			_lock.EnterWriteLock();
			try
			{
				foreach (var i in _mappers.Where(kvp => kvp.Value == mapper).ToList())
					_mappers.Remove(i.Key);
			}
			finally
			{
				_lock.ExitWriteLock();
				FlushCaches();
			}
		}

		public static IMapper GetMapper(Type t)
		{
			_lock.EnterReadLock();
			try
			{
				IMapper val;
				if (_mappers.TryGetValue(t, out val))
					return val;
				if (_mappers.TryGetValue(t.Assembly, out val))
					return val;

				return Singleton<StandardMapper>.Instance;
			}
			finally
			{
				_lock.ExitReadLock();
			}
		}

		static void RegisterInternal(object typeOrAssembly, IMapper mapper)
		{
			_lock.EnterWriteLock();
			try
			{
				_mappers.Add(typeOrAssembly, mapper);
			}
			finally
			{
				_lock.ExitWriteLock();
				FlushCaches();
			}
		}

		static void RevokeInternal(object typeOrAssembly)
		{
			_lock.EnterWriteLock();
			try
			{
				_mappers.Remove(typeOrAssembly);
			}
			finally
			{
				_lock.ExitWriteLock();
				FlushCaches();
			}
		}

		static void FlushCaches()
		{
			MultiPocoFactory.FlushCaches();
			PocoData.FlushCaches();
		}

		static Dictionary<object, IMapper> _mappers = new Dictionary<object,IMapper>();
		static ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();
	}
}
