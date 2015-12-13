// <copyright file="SqlServerDatabaseType.cs" company="PetaPoco - CollaboratingPlatypus">
//      Apache License, Version 2.0 https://github.com/CollaboratingPlatypus/PetaPoco/blob/master/LICENSE.txt
// </copyright>
// <author>PetaPoco - CollaboratingPlatypus</author>
// <date>2015/12/05</date>

using System;
using System.Linq;
using PetaPoco.Core;
using PetaPoco.Internal;
using PetaPoco.Utilities;

namespace PetaPoco.DatabaseTypes
{
    public class SqlServerDatabaseType : DatabaseType
    {
        public override string BuildPageQuery(long skip, long take, SQLParts parts, ref object[] args)
        {
            var helper = (PagingHelper) PagingHelper;
            parts.SqlSelectRemoved = helper.RegexOrderBy.Replace(parts.SqlSelectRemoved, "", 1);
            if (helper.RegexDistinct.IsMatch(parts.SqlSelectRemoved))
            {
                parts.SqlSelectRemoved = "peta_inner.* FROM (SELECT " + parts.SqlSelectRemoved + ") peta_inner";
            }
            var sqlPage = string.Format("SELECT * FROM (SELECT ROW_NUMBER() OVER ({0}) peta_rn, {1}) peta_paged WHERE peta_rn>@{2} AND peta_rn<=@{3}",
                parts.SqlOrderBy == null ? "ORDER BY (SELECT NULL)" : parts.SqlOrderBy, parts.SqlSelectRemoved, args.Length, args.Length + 1);
            args = args.Concat(new object[] {skip, skip + take}).ToArray();

            return sqlPage;
        }

        public override object ExecuteInsert(Database db, System.Data.IDbCommand cmd, string primaryKeyName)
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