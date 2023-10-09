using PetaPoco.Tests.Integration.Providers;
using Xunit;

namespace PetaPoco.Tests.Integration.Databases.SqlServerCe
{
    [Collection("SqlServerCe")]
    public class SqlServerCeQueryLinqTests : QueryLinqTests
    {
        public SqlServerCeQueryLinqTests()
            : base(new SqlServerCeTestProvider())
        {
        }
    }
}
