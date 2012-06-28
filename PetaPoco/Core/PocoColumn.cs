// PetaPoco - A Tiny ORMish thing for your POCO's.
// Copyright © 2011-2012 Topten Software.  All Rights Reserved.
 
using System;
using System.Reflection;

namespace PetaPoco.Internal
{
	internal class PocoColumn
	{
		public string ColumnName;
		public PropertyInfo PropertyInfo;
		public bool ResultColumn;
		public bool ForceToUtc;
		public virtual void SetValue(object target, object val) { PropertyInfo.SetValue(target, val, null); }
		public virtual object GetValue(object target) { return PropertyInfo.GetValue(target, null); }
		public virtual object ChangeType(object val) { return Convert.ChangeType(val, PropertyInfo.PropertyType); }
	}
}
