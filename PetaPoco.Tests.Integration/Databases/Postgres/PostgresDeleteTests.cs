using Xunit;

namespace PetaPoco.Tests.Integration.Databases.Postgres
{
    [Collection("Postgres")]
    [Trait("Category", "Postgres")]
    public class PostgresDeleteTests : BaseDeleteTests
    {
        public PostgresDeleteTests()
            : base(new PostgresDBTestProvider())
        {
        }
    }
}