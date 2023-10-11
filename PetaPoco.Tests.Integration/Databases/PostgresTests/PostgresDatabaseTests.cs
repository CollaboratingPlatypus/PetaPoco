using PetaPoco.Tests.Integration.Providers;
using Xunit;

namespace PetaPoco.Tests.Integration.Databases.Postgres
{
    [Collection("Postgres")]
    public class PostgresDatabaseTests : DatabaseTests
    {
        public PostgresDatabaseTests()
            : base(new PostgresTestProvider())
        {
        }
    }
}
