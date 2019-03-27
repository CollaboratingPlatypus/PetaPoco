using Xunit;

namespace PetaPoco.Tests.Integration.Databases.Postgres
{
    [Collection("Postgres")]
    public class PostgresDeleteTests : BaseDeleteTests
    {
        public PostgresDeleteTests()
            : base(new PostgresDBTestProvider())
        {
        }
    }
}