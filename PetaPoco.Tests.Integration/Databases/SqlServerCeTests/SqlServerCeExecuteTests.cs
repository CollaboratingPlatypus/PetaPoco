using PetaPoco.Tests.Integration.Providers;
using Xunit;

namespace PetaPoco.Tests.Integration.Databases.SqlServerCe
{
    [Collection("SqlServerCe")]
    public class SqlServerCeExecuteTests : ExecuteTests
    {
        public SqlServerCeExecuteTests()
            : base(new SqlServerCeTestProvider())
        {
        }
    }
}
