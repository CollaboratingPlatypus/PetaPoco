using Xunit;

namespace PetaPoco.Tests.Integration.Databases.SqlServer
{
    [Collection("SqlServer")]
    public class SqlServerMiscellaneousTests : MiscellaneousTests
    {
        public SqlServerMiscellaneousTests()
            : base(new SqlServerDbProviderFactory())
        {
        }
    }
}
