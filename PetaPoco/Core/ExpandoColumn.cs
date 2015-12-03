// PetaPoco - A Tiny ORMish thing for your POCO's.
// Copyright © 2011-2012 Topten Software.  All Rights Reserved.
 
using System;
using System.Collections.Generic;

namespace PetaPoco.Internal
{
	internal class ExpandoColumn : PocoColumn
	{
		public override void SetValue(object target, object val) { (target as IDictionary<string, object>)[ColumnName] = val; }
		public override object GetValue(object target)
		{
			object val = null;
			(target as IDictionary<string, object>).TryGetValue(ColumnName, out val);
			return val;
		}
		public override object ChangeType(object val) { return val; }
	}

}
