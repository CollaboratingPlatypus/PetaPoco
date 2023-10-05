using Xunit;

namespace PetaPoco.Tests.Integration.Databases.SqlServer
{
    [Collection("SqlServer")]
    public class SqlServerDeleteTests : DeleteTests
    {
        public SqlServerDeleteTests()
            : base(new SqlServerDbProviderFactory())
        {
        }
    }
}
