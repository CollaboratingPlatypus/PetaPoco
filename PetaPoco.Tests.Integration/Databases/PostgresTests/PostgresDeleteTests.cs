using PetaPoco.Tests.Integration.Providers;
using Xunit;

namespace PetaPoco.Tests.Integration.Databases.Postgres
{
    [Collection("Postgres")]
    public class PostgresDeleteTests : DeleteTests
    {
        public PostgresDeleteTests()
            : base(new PostgresTestProvider())
        {
        }
    }
}
