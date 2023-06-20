using System.Data.Common;
using PetaPoco.Core;

namespace PetaPoco.Providers
{
    /// <summary>
    /// The SqlServerMsDataDatabaseProvider class provides a specific implementation of the <see cref="DatabaseProvider"/> class for the SQL Server database.
    /// </summary>
    /// <remarks>
    /// Under the hood, this class serves as a wrapper for the <see cref="SqlServerDatabaseProvider"/> class.
    /// </remarks>
    public class SqlServerMsDataDatabaseProvider : SqlServerDatabaseProvider
    {
        /// <inheritdoc />
        public override DbProviderFactory GetFactory()
            => GetFactory("Microsoft.Data.SqlClient.SqlClientFactory, Microsoft.Data.SqlClient");
    }
}
