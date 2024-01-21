using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using PetaPoco.Core;
using PetaPoco.Utilities;

namespace PetaPoco.Providers
{
    // TODO: Rename class: SqlServerCeDatabaseProvider

    /// <summary>
    /// Provides an implementation of <see cref="DatabaseProvider"/> for Microsoft SQL Server Compact Edition databases.
    /// </summary>
    /// <remarks>
    /// This provider uses the "System.Data.SqlServerCe" ADO.NET driver for data access. Note that this driver is end-of-life (<a
    /// href="https://learn.microsoft.com/en-us/lifecycle/products/microsoft-sql-server-compact-40/">reference</a>).
    /// </remarks>
    public class SqlServerCEDatabaseProviders : DatabaseProvider
    {
        /// <inheritdoc/>
        public override DbProviderFactory GetFactory()
            => GetFactory("System.Data.SqlServerCe.SqlCeProviderFactory, System.Data.SqlServerCe, Culture=neutral, PublicKeyToken=89845dcd8080cc91");

        /// <inheritdoc/>
        public override string BuildPageQuery(long skip, long take, SQLParts parts, ref object[] args)
        {
            if (string.IsNullOrEmpty(parts.SqlOrderBy))
                parts.Sql += " ORDER BY ABS(1)";
            var sqlPage = $"{parts.Sql}\nOFFSET @{args.Length} ROWS FETCH NEXT @{args.Length + 1} ROWS ONLY";
            args = args.Concat(new object[] { skip, take }).ToArray();
            return sqlPage;
        }

        /// <inheritdoc/>
        public override object ExecuteInsert(Database db, IDbCommand cmd, string primaryKeyName)
        {
            ExecuteNonQueryHelper(db, cmd);
            return db.ExecuteScalar<object>("SELECT @@@IDENTITY AS NewID;");
        }

#if ASYNC
        /// <inheritdoc/>
        public override async Task<object> ExecuteInsertAsync(CancellationToken cancellationToken, Database db, IDbCommand cmd, string primaryKeyName)
        {
            await ExecuteNonQueryHelperAsync(cancellationToken, db, cmd);
            return await db.ExecuteScalarAsync<object>(cancellationToken, "SELECT @@@IDENTITY AS NewID;");
        }
#endif
    }
}
