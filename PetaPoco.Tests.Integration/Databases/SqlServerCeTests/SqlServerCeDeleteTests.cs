using PetaPoco.Tests.Integration.Providers;
using Xunit;

namespace PetaPoco.Tests.Integration.Databases.SqlServerCe
{
    [Collection("SqlServerCe")]
    public class SqlServerCeDeleteTests : DeleteTests
    {
        public SqlServerCeDeleteTests()
            : base(new SqlServerCeTestProvider())
        {
        }
    }
}
