﻿using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using PetaPoco.Core;
using PetaPoco.Utilities;

namespace PetaPoco.Providers
{
    /// <summary>
    /// Provides an implementation of <see cref="DatabaseProvider"/> for Microsoft SQL Server databases.
    /// </summary>
    /// <remarks>
    /// This provider uses the "System.Data.SqlClient" ADO.NET driver for data access. Note that this driver is end-of-life (<see
    /// href="https://learn.microsoft.com/en-us/sql/connect/ado-net/introduction-microsoft-data-sqlclient-namespace/">reference</see> and
    /// <see href="https://www.connectionstrings.com/the-new-microsoft-data-sqlclient-explained/">more info</see>). Consider using <see
    /// cref="SqlServerMsDataDatabaseProvider"/> as an alternative, which uses the "Microsoft.Data.SqlClient" driver.
    /// </remarks>
    public class SqlServerDatabaseProvider : DatabaseProvider
    {
        /// <inheritdoc/>
        public override DbProviderFactory GetFactory()
            => GetFactory("System.Data.SqlClient.SqlClientFactory, System.Data, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089");

        /// <inheritdoc/>
        public override string BuildPageQuery(long skip, long take, SQLParts parts, ref object[] args)
        {
            var helper = (PagingHelper)PagingUtility;
            // when the query does not contain an "order by", it is very slow
            if (helper.SimpleRegexOrderBy.IsMatch(parts.SqlSelectRemoved))
            {
                var m = helper.SimpleRegexOrderBy.Match(parts.SqlSelectRemoved);
                if (m.Success)
                {
                    var g = m.Groups[0];
                    parts.SqlSelectRemoved = parts.SqlSelectRemoved.Substring(0, g.Index);
                }
            }

            if (helper.RegexDistinct.IsMatch(parts.SqlSelectRemoved))
                parts.SqlSelectRemoved = "peta_inner.* FROM (SELECT " + parts.SqlSelectRemoved + ") peta_inner";

            var sqlPage =
                $"SELECT * FROM (SELECT ROW_NUMBER() OVER ({parts.SqlOrderBy ?? "ORDER BY (SELECT NULL)"}) peta_rn, {parts.SqlSelectRemoved}) peta_paged WHERE peta_rn > @{args.Length} AND peta_rn <= @{args.Length + 1}";
            args = args.Concat(new object[] { skip, skip + take }).ToArray();
            return sqlPage;
        }

        /// <inheritdoc/>
        public override object ExecuteInsert(Database db, IDbCommand cmd, string primaryKeyName)
            => ExecuteScalarHelper(db, cmd);

#if ASYNC
        /// <inheritdoc/>
        public override Task<object> ExecuteInsertAsync(CancellationToken cancellationToken, Database db, IDbCommand cmd, string primaryKeyName)
            => ExecuteScalarHelperAsync(cancellationToken, db, cmd);
#endif

        /// <inheritdoc/>
        public override string GetExistsSql()
            => "IF EXISTS (SELECT 1 FROM {0} WHERE {1}) SELECT 1 ELSE SELECT 0";

        /// <inheritdoc/>
        public override string GetInsertOutputClause(string primaryKeyName)
            => $" OUTPUT INSERTED.[{primaryKeyName}]";
    }
}
