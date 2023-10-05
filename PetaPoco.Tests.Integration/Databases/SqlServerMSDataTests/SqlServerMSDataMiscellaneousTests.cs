using Xunit;

namespace PetaPoco.Tests.Integration.Databases.SqlServerMSData
{
    [Collection("SqlServerMSData")]
    public class SqlServerMSDataMiscellaneousTests : MiscellaneousTests
    {
        public SqlServerMSDataMiscellaneousTests()
            : base(new SqlServerMSDataDbProviderFactory())
        {
        }
    }
}
