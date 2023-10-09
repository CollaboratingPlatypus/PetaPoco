using System.Data.Common;
using PetaPoco.Core;

namespace PetaPoco.Providers
{
    // TODO: Rename class: SqlServerMSDataDatabaseProvider

    /// <summary>
    /// Provides an implementation of <see cref="DatabaseProvider"/> for Microsoft SQL Server databases.
    /// </summary>
    /// <remarks>
    /// This provider uses the "Microsoft.Data.SqlClient" ADO.NET driver for data access.
    /// </remarks>
    public class SqlServerMsDataDatabaseProvider : SqlServerDatabaseProvider
    {
        /// <inheritdoc/>
        public override DbProviderFactory GetFactory()
            => GetFactory("Microsoft.Data.SqlClient.SqlClientFactory, Microsoft.Data.SqlClient");
    }
}
