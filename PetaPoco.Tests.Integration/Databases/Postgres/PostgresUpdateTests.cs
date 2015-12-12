using Xunit;

namespace PetaPoco.Tests.Integration.Databases.Postgres
{
    [Collection("PostgresTests")]
    public class PostgresUpdateTests : BaseUpdateTests
    {
        public PostgresUpdateTests()
            : base(new PostgresDBTestProvider())
        {
        }
    }
}