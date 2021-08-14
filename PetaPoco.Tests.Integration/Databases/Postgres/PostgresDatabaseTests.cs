using Xunit;

namespace PetaPoco.Tests.Integration.Databases.Postgres
{
    [Collection("Postgres")]
    [Trait("Category", "Postgres")]
    public class PostgresDatabaseTests : BaseDatabaseTests
    {
        public PostgresDatabaseTests()
            : base(new PostgresDBTestProvider())
        {
        }
    }
}