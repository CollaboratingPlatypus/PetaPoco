using Xunit;

namespace PetaPoco.Tests.Integration.Databases.SqlServerMSData
{
    [Collection("SqlServerMSData")]
    public class SqlServerMSDataDeleteTests : DeleteTests
    {
        public SqlServerMSDataDeleteTests()
            : base(new SqlServerMSDataDbProviderFactory())
        {
        }
    }
}
