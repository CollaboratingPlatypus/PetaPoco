using Xunit;

namespace PetaPoco.Tests.Integration.Databases.SqlServer
{
    [Collection("SqlServer")]
    public class SqlServerDatabaseTests : DatabaseTests
    {
        public SqlServerDatabaseTests()
            : base(new SqlServerDbProviderFactory())
        {
        }
    }
}
