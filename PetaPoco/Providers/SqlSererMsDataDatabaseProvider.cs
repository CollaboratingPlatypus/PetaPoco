using System.Data.Common;

namespace PetaPoco.Providers
{
    public class SqlSererMsDataDatabaseProvider : SqlServerDatabaseProvider
    {
        public override DbProviderFactory GetFactory()
            => GetFactory("Microsoft.Data.SqlClient.SqlClientFactory, Microsoft.Data.SqlClient");
    }
}