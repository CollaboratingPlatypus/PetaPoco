using PetaPoco.Tests.Integration.Providers;
using Xunit;

namespace PetaPoco.Tests.Integration.Databases.SqlServerCe
{
    [Collection("SqlServerCe")]
    public class SqlServerCeMiscellaneousTests : MiscellaneousTests
    {
        public SqlServerCeMiscellaneousTests()
            : base(new SqlServerCeTestProvider())
        {
        }
    }
}
