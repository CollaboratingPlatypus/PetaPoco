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
    /// <summary>
    /// The MsAccessDbDatabaseProvider class provides a specific implementation of the <see cref="DatabaseProvider"/> class for the Microsoft Access database.
    /// </summary>
    public class MsAccessDbDatabaseProvider : DatabaseProvider
    {
        /// <inheritdoc />
        public override DbProviderFactory GetFactory()
            => GetFactory("System.Data.OleDb.OleDbFactory, System.Data, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089");

        /// <inheritdoc />
        public override object ExecuteInsert(Database database, IDbCommand cmd, string primaryKeyName)
        {
            ExecuteNonQueryHelper(database, cmd);
            cmd.CommandText = "SELECT @@IDENTITY AS NewID;";
            return ExecuteScalarHelper(database, cmd);
        }

#if ASYNC
        /// <inheritdoc />
        public override async Task<object> ExecuteInsertAsync(CancellationToken cancellationToken, Database database, IDbCommand cmd, string primaryKeyName)
        {
            await ExecuteNonQueryHelperAsync(cancellationToken, database, cmd).ConfigureAwait(false);
            cmd.CommandText = "SELECT @@IDENTITY AS NewID;";
            return await ExecuteScalarHelperAsync(cancellationToken, database, cmd).ConfigureAwait(false);
        }
#endif

        /// <summary>
        /// Page queries are not supported by MsAccess database.
        /// </summary>
        /// <returns>This method always throws a <see cref="NotSupportedException"/>.</returns>
        /// <exception cref="NotSupportedException">Always thrown because the Access provider does not support paging.</exception>
        /// <inheritdoc />
        public override string BuildPageQuery(long skip, long take, SQLParts parts, ref object[] args)
            => throw new NotSupportedException("The MS Access provider does not support paging.");
    }
}
