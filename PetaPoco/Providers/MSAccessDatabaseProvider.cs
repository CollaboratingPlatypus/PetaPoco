using System;
using System.Data;
using System.Data.Common;
#if ASYNC
using System.Threading;
using System.Threading.Tasks;
#endif
using PetaPoco.Core;
using PetaPoco.Utilities;

namespace PetaPoco.Providers
{
    // TODO: Rename class: MSAccessDatabaseProvider

    /// <summary>
    /// Provides an implementation of <see cref="DatabaseProvider"/> for Microsoft Access databases.
    /// </summary>
    /// <remarks>
    /// This provider uses the "System.Data.OleDb" ADO.NET driver for data access.
    /// </remarks>
    public class MsAccessDbDatabaseProvider : DatabaseProvider
    {
        /// <inheritdoc/>
        public override DbProviderFactory GetFactory()
            => GetFactory("System.Data.OleDb.OleDbFactory, System.Data, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089");

        /// <inheritdoc/>
        public override object ExecuteInsert(Database db, IDbCommand cmd, string primaryKeyName)
        {
            ExecuteNonQueryHelper(db, cmd);
            cmd.CommandText = "SELECT @@IDENTITY AS NewID;";
            return ExecuteScalarHelper(db, cmd);
        }

#if ASYNC
        /// <inheritdoc/>
        public override async Task<object> ExecuteInsertAsync(CancellationToken cancellationToken, Database db, IDbCommand cmd, string primaryKeyName)
        {
            await ExecuteNonQueryHelperAsync(cancellationToken, db, cmd).ConfigureAwait(false);
            cmd.CommandText = "SELECT @@IDENTITY AS NewID;";
            return await ExecuteScalarHelperAsync(cancellationToken, db, cmd).ConfigureAwait(false);
        }
#endif

        /// <summary>
        /// Page queries are not supported by MS Access database.
        /// </summary>
        /// <returns>This method always throws a <see cref="NotSupportedException"/>.</returns>
        /// <exception cref="NotSupportedException">The MS Access provider does not support paging.</exception>
        /// <inheritdoc/>
        public override string BuildPageQuery(long skip, long take, SQLParts parts, ref object[] args)
            => throw new NotSupportedException("The MS Access provider does not support paging.");
    }
}
