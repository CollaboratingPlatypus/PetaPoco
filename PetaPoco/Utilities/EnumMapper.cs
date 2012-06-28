using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PetaPoco.Internal
{
	internal static class EnumMapper
	{
		public static object EnumFromString(Type enumType, string value)
		{
			Dictionary<string, object> map = _types.GetOrAdd(enumType, (_) =>
			{
				var values = Enum.GetValues(enumType);

				var newmap = new Dictionary<string, object>(values.Length, StringComparer.InvariantCultureIgnoreCase);

				foreach (var v in values)
				{
					newmap.Add(v.ToString(), v);
				}

				return newmap;
			});


			return map[value];
		}

		static ConcurrentDictionary<Type, Dictionary<string, object>> _types = new ConcurrentDictionary<Type,Dictionary<string,object>>();
	}
}
