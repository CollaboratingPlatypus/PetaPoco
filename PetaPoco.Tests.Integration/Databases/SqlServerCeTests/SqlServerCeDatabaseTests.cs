using PetaPoco.Tests.Integration.Providers;
using Xunit;

namespace PetaPoco.Tests.Integration.Databases.SqlServerCe
{
    [Collection("SqlServerCe")]
    public class SqlServerCeDatabaseTests : DatabaseTests
    {
        public SqlServerCeDatabaseTests()
            : base(new SqlServerCeTestProvider())
        {
        }
    }
}
