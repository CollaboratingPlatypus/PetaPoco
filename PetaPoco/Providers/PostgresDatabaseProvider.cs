using System.Data;
using System.Data.Common;
using PetaPoco.Core;
#if ASYNC
using System.Threading;
using System.Threading.Tasks;
#endif

namespace PetaPoco.Providers
{
    // TODO: Rename class: PostgresDatabaseProvider

    /// <summary>
    /// Provides an implementation of <see cref="DatabaseProvider"/> for PostgreSQL databases.
    /// </summary>
    /// <remarks>
    /// This provider uses the "Npgsql" ADO.NET driver for data access.
    /// </remarks>
    public class PostgreSQLDatabaseProvider : DatabaseProvider
    {
        /// <inheritdoc/>
        public override bool HasNativeGuidSupport => true;

        /// <inheritdoc/>
        public override DbProviderFactory GetFactory()
            => GetFactory("Npgsql.NpgsqlFactory, Npgsql, Culture=neutral, PublicKeyToken=5d8b90d52f46fda7");

        /// <inheritdoc/>
        public override string GetExistsSql() => "SELECT CASE WHEN EXISTS(SELECT 1 FROM {0} WHERE {1}) THEN 1 ELSE 0 END";

        /// <inheritdoc/>
        public override object MapParameterValue(object value)
        {
            // Don't map bools to ints in PostgreSQL
            if (value is bool)
                return value;

            return base.MapParameterValue(value);
        }

        /// <inheritdoc/>
        public override string EscapeSqlIdentifier(string sqlIdentifier) => $"\"{sqlIdentifier}\"";

        /// <inheritdoc/>
        public override object ExecuteInsert(Database db, IDbCommand cmd, string primaryKeyName)
        {
            if (primaryKeyName != null)
            {
                cmd.CommandText += $"returning {EscapeSqlIdentifier(primaryKeyName)} as NewID";
                return ExecuteScalarHelper(db, cmd);
            }

            ExecuteNonQueryHelper(db, cmd);
            return -1;
        }

#if ASYNC
        /// <inheritdoc/>
        public override async Task<object> ExecuteInsertAsync(CancellationToken cancellationToken, Database db, IDbCommand cmd, string primaryKeyName)
        {
            if (primaryKeyName != null)
            {
                cmd.CommandText += $"returning {EscapeSqlIdentifier(primaryKeyName)} as NewID";
                return await ExecuteScalarHelperAsync(cancellationToken, db, cmd).ConfigureAwait(false);
            }

            await ExecuteNonQueryHelperAsync(cancellationToken, db, cmd);
            return -1;
        }
#endif
    }
}
