using System;
using System.Data.Common;
using PetaPoco.Core;

namespace PetaPoco.Providers
{
    /// <summary>
    /// Provides an implementation of <see cref="DatabaseProvider"/> for MySQL databases.
    /// </summary>
    /// <remarks>
    /// This provider uses the "MySql.Data.MySqlClient" ADO.NET driver for data access.
    /// </remarks>
    public class MySqlDatabaseProvider : DatabaseProvider
    {
        /// <inheritdoc/>
        public override DbProviderFactory GetFactory()
            => GetFactory("MySql.Data.MySqlClient.MySqlClientFactory, MySql.Data, Culture=neutral, PublicKeyToken=c5687fc88969c44d");

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
