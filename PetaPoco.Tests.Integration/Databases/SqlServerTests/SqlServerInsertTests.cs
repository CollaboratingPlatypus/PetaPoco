using Xunit;

namespace PetaPoco.Tests.Integration.Databases.SqlServer
{
    [Collection("SqlServer")]
    public class SqlServerInsertTests : InsertTests
    {
        public SqlServerInsertTests()
            : base(new SqlServerDbProviderFactory())
        {
        }
    }
}
