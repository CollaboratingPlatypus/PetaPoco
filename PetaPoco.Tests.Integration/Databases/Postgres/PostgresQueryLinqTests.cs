using Xunit;

namespace PetaPoco.Tests.Integration.Databases.Postgres
{
    [Collection("Postgres")]
    [Trait("Category", "Postgres")]
    public class PostgresQueryLinqTests : BaseQueryLinqTests
    {
        public PostgresQueryLinqTests()
            : base(new PostgresDBTestProvider())
        {
        }
    }
}