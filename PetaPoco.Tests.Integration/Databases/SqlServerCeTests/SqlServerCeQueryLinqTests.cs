using Xunit;

namespace PetaPoco.Tests.Integration.Databases.SqlServerCe
{
    [Collection("SqlServerCe")]
    public class SqlServerCeQueryLinqTests : QueryLinqTests
    {
        public SqlServerCeQueryLinqTests()
            : base(new SqlServerCeDbProviderFactory())
        {
        }
    }
}
