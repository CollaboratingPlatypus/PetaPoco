using Xunit;

namespace PetaPoco.Tests.Integration.Databases.SqlServerMSData
{
    [Collection("SqlServerMSData")]
    public class SqlServerMSDataExecuteTests : ExecuteTests
    {
        public SqlServerMSDataExecuteTests()
            : base(new SqlServerMSDataDbProviderFactory())
        {
        }
    }
}
