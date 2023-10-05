using Xunit;

namespace PetaPoco.Tests.Integration.Databases.SqlServerMSData
{
    [Collection("SqlServerMSData")]
    public class SqlServerMSDataDatabaseTests : DatabaseTests
    {
        public SqlServerMSDataDatabaseTests()
            : base(new SqlServerMSDataDbProviderFactory())
        {
        }
    }
}
