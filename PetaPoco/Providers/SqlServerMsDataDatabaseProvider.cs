using System.Data.Common;
using PetaPoco.Core;

namespace PetaPoco.Providers
{
    /// <summary>
    /// Provides a specific implementation of the <see cref="DatabaseProvider"/> class for Microsoft.Data.SqlClient.
    /// </summary>
    /// <remarks>
    /// This class serves as a wrapper for the <see cref="SqlServerDatabaseProvider"/> class.
    /// </remarks>
    public class SqlServerMsDataDatabaseProvider : SqlServerDatabaseProvider
    {
        /// <inheritdoc/>
        public override DbProviderFactory GetFactory()
            => GetFactory("Microsoft.Data.SqlClient.SqlClientFactory, Microsoft.Data.SqlClient");
    }
}
