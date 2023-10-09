using PetaPoco.Tests.Integration.Providers;
using Xunit;

namespace PetaPoco.Tests.Integration.Databases.SqlServerCe
{
    [Collection("SqlServerCe")]
    public class SqlServerCeUpdateTests : UpdateTests
    {
        public SqlServerCeUpdateTests()
            : base(new SqlServerCeTestProvider())
        {
        }
    }
}
