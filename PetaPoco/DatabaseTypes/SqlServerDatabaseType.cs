// PetaPoco - A Tiny ORMish thing for your POCO's.
// Copyright © 2011-2012 Topten Software.  All Rights Reserved.

using System;
using System.Linq;
using System.Text.RegularExpressions;
using PetaPoco.Internal;

namespace PetaPoco.DatabaseTypes
{
	class SqlServerDatabaseType : DatabaseType
	{
		public override string BuildPageQuery(long skip, long take, PagingHelper.SQLParts parts, ref object[] args)
		{
			parts.sqlSelectRemoved = PagingHelper.rxOrderBy.Replace(parts.sqlSelectRemoved, "", 1);
			if (PagingHelper.rxDistinct.IsMatch(parts.sqlSelectRemoved))
			{
				parts.sqlSelectRemoved = "peta_inner.* FROM (SELECT " + parts.sqlSelectRemoved + ") peta_inner";
			}
			var sqlPage = string.Format("SELECT * FROM (SELECT ROW_NUMBER() OVER ({0}) peta_rn, {1}) peta_paged WHERE peta_rn>@{2} AND peta_rn<=@{3}",
									parts.sqlOrderBy == null ? "ORDER BY (SELECT NULL)" : parts.sqlOrderBy, parts.sqlSelectRemoved, args.Length, args.Length + 1);
			args = args.Concat(new object[] { skip, skip + take }).ToArray();

			return sqlPage;
		}

		public override object ExecuteInsert(Database db, System.Data.IDbCommand cmd, string PrimaryKeyName)
		{
			return db.ExecuteScalarHelper(cmd);
		}

		public override string GetExistsSql()
		{
			return "IF EXISTS (SELECT 1 FROM {0} WHERE {1}) SELECT 1 ELSE SELECT 0";
		}

		public override string GetInsertOutputClause(string primaryKeyName)
		{
			return String.Format(" OUTPUT INSERTED.[{0}]", primaryKeyName);
		}
	}

}
