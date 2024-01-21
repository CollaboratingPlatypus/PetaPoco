using System;
using System.Data.Common;
using PetaPoco.Core;

namespace PetaPoco.Providers
{
    /// <summary>
    /// Provides an implementation of <see cref="DatabaseProvider"/> for MySQL databases using the MySqlConnector library.
    /// </summary>
    /// <remarks>
    /// This provider uses the "MySqlConnector" ADO.NET driver for data access.
    /// </remarks>
    public class MySqlConnectorDatabaseProvider : DatabaseProvider
    {
        /// <inheritdoc/>
        public override DbProviderFactory GetFactory() => GetFactory("MySqlConnector.MySqlConnectorFactory, MySqlConnector");

        /// <inheritdoc/>
        public override string GetParameterPrefix(string connectionString)
        {
            if (connectionString != null && connectionString.IndexOf("Allow User Variables=true", StringComparison.Ordinal) >= 0)
                return "?";
            return "@";
        }

        /// <inheritdoc/>
        public override string EscapeSqlIdentifier(string sqlIdentifier) => $"`{sqlIdentifier}`";

        /// <inheritdoc/>
        public override string GetExistsSql() => "SELECT EXISTS (SELECT 1 FROM {0} WHERE {1})";
    }
}
