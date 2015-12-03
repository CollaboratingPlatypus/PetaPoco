// PetaPoco - A Tiny ORMish thing for your POCO's.
// Copyright © 2011-2012 Topten Software.  All Rights Reserved.

using System;
using System.Linq;
using PetaPoco.Internal;

namespace PetaPoco.DatabaseTypes
{
	class SqlServerCEDatabaseType : DatabaseType
	{
		public override string BuildPageQuery(long skip, long take, PagingHelper.SQLParts parts, ref object[] args)
		{
			var	sqlPage = string.Format("{0}\nOFFSET @{1} ROWS FETCH NEXT @{2} ROWS ONLY", parts.sql, args.Length, args.Length + 1);
			args = args.Concat(new object[] { skip, take }).ToArray();
			return sqlPage;
		}

		public override object ExecuteInsert(Database db, System.Data.IDbCommand cmd, string PrimaryKeyName)
		{
			db.ExecuteNonQueryHelper(cmd);
			return db.ExecuteScalar<object>("SELECT @@@IDENTITY AS NewID;");
		}

	}
}
