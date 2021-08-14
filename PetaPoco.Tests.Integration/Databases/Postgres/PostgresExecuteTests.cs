using Xunit;

namespace PetaPoco.Tests.Integration.Databases.Postgres
{
    [Collection("Postgres")]
    [Trait("Category", "Postgres")]
    public class PostgresExecuteTests : BaseExecuteTests
    {
        public PostgresExecuteTests()
            : base(new PostgresDBTestProvider())
        {
        }
    }
}