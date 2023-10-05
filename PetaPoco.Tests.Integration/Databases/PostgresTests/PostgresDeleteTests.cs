using Xunit;

namespace PetaPoco.Tests.Integration.Databases.Postgres
{
    [Collection("Postgres")]
    public class PostgresDeleteTests : DeleteTests
    {
        public PostgresDeleteTests()
            : base(new PostgresDbProviderFactory())
        {
        }
    }
}
