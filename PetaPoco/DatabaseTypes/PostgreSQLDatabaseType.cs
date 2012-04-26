// PetaPoco - A Tiny ORMish thing for your POCO's.
// Copyright © 2011-2012 Topten Software.  All Rights Reserved.

using System;


namespace PetaPoco
{
	class PostgreSQLDatabaseType : DatabaseType
	{
		public override bool MapBoolToInteger()
		{
			return false;
		}

		public override string EscapeSqlIdentifier(string str)
		{
			return string.Format("\"{0}\"", str);
		}

		public override object ExecuteInsert(Database db, System.Data.IDbCommand cmd, string PrimaryKeyName)
		{
			if (PrimaryKeyName != null)
			{
				cmd.CommandText += string.Format("returning {0} as NewID", EscapeSqlIdentifier(PrimaryKeyName));
				return db.ExecuteScalarHelper(cmd);
			}
			else
			{
				db.ExecuteNonQueryHelper(cmd);
				return -1;
			}
		}
	}
}
