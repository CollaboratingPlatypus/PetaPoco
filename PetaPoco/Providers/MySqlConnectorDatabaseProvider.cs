using System;
using System.Data.Common;
using PetaPoco.Core;

namespace PetaPoco.Providers
{
    public class MySqlConnectorDatabaseProvider : DatabaseProvider
    {
        public override DbProviderFactory GetFactory()
            => GetFactory("MySqlConnector.MySqlConnectorFactory, MySqlConnector");

        public override string GetParameterPrefix(string connectionString)
        {
            if (connectionString != null && connectionString.IndexOf("Allow User Variables=true", StringComparison.Ordinal) >= 0)
                return "?";
            return "@";
        }

        public override string EscapeSqlIdentifier(string sqlIdentifier)
            => $"`{sqlIdentifier}`";

        public override string GetExistsSql()
            => "SELECT EXISTS (SELECT 1 FROM {0} WHERE {1})";
    }
}