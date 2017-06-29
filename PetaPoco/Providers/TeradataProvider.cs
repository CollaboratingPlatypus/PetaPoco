using System;
using System.Data;
using System.Data.Common;
using PetaPoco.Core;
using PetaPoco.Internal;
using PetaPoco.Utilities;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;

namespace PetaPoco.Providers
{
    public class TeradataDatabaseProvider : DatabaseProvider
    {
        public override DbProviderFactory GetFactory()
        {
            return DbProviderFactories.GetFactory("Teradata.Client.Provider");
        }

        private static readonly Regex simpleRegexOrderBy = new Regex(@"\bORDER\s+BY\s+", RegexOptions.RightToLeft | RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.Compiled);

        public override string BuildPageQuery(long skip, long take, SQLParts parts, ref object[] args)
        {
            var helper = (PagingHelper)PagingUtility;
            // when the query does not contain an "order by", it is very slow
            if (simpleRegexOrderBy.IsMatch(parts.SqlSelectRemoved))
            {
                parts.SqlSelectRemoved = helper.RegexOrderBy.Replace(parts.SqlSelectRemoved, "", 1);
            }
            if (helper.RegexDistinct.IsMatch(parts.SqlSelectRemoved))
            {
                parts.SqlSelectRemoved = "peta_inner.* FROM (SELECT " + parts.SqlSelectRemoved + ") peta_inner";
            }
            var sqlPage = string.Format("SELECT * FROM (SELECT ROW_NUMBER() OVER ({0}) peta_rn, {1}) peta_paged WHERE peta_rn > @{2} AND peta_rn <= @{3}", parts.SqlOrderBy ?? "ORDER BY (SELECT NULL)", parts.SqlSelectRemoved, args.Length, args.Length + 1);
            args = args.Concat(new object[] { skip, skip + take }).ToArray();
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
