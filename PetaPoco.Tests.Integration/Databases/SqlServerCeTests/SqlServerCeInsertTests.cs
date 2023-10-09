using PetaPoco.Tests.Integration.Providers;
using Xunit;

namespace PetaPoco.Tests.Integration.Databases.SqlServerCe
{
    [Collection("SqlServerCe")]
    public class SqlServerCeInsertTests : InsertTests
    {
        public SqlServerCeInsertTests()
            : base(new SqlServerCeTestProvider())
        {
        }
    }
}
