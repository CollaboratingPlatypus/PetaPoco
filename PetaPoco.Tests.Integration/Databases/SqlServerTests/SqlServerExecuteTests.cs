using Xunit;

namespace PetaPoco.Tests.Integration.Databases.SqlServer
{
    [Collection("SqlServer")]
    public class SqlServerExecuteTests : ExecuteTests
    {
        public SqlServerExecuteTests()
            : base(new SqlServerDbProviderFactory())
        {
        }
    }
}
