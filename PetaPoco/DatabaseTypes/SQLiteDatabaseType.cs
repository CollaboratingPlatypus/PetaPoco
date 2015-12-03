// PetaPoco - A Tiny ORMish thing for your POCO's.
// Copyright © 2011-2012 Topten Software.  All Rights Reserved.

using System;
using PetaPoco.Internal;


namespace PetaPoco.DatabaseTypes
{
	class SQLiteDatabaseType : DatabaseType
	{
		public override object MapParameterValue(object value)
		{
			if (value.GetType() == typeof(uint))
				return (long)((uint)value);

			return base.MapParameterValue(value);
		}

		public override object ExecuteInsert(Database db, System.Data.IDbCommand cmd, string PrimaryKeyName)
		{
			if (PrimaryKeyName != null)
			{
				cmd.CommandText += ";\nSELECT last_insert_rowid();";
				return db.ExecuteScalarHelper(cmd);
			}
			else
			{
				db.ExecuteNonQueryHelper(cmd);
				return -1;
			}
		}

		public override string GetExistsSql()
		{
			return "SELECT EXISTS (SELECT 1 FROM {0} WHERE {1})";
		}

	}
}
