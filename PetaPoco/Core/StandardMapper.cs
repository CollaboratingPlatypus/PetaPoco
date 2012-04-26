using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace PetaPoco
{
	public class StandardMapper : IMapper
	{
		public TableInfo GetTableInfo(Type pocoType)
		{
			return TableInfo.FromPoco(pocoType);
		}

		public ColumnInfo GetColumnInfo(PropertyInfo pocoProperty)
		{
			return ColumnInfo.FromProperty(pocoProperty);
		}

		public Func<object, object> GetFromDbConverter(PropertyInfo TargetProperty, Type SourceType)
		{
			return null;
		}

		public Func<object, object> GetToDbConverter(PropertyInfo SourceProperty)
		{
			return null;
		}
	}
}
