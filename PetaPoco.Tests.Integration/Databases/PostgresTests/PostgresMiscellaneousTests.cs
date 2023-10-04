using Xunit;

namespace PetaPoco.Tests.Integration.Databases.Postgres
{
    [Collection("Postgres")]
    public class PostgresMiscellaneousTests : BaseMiscellaneousTests
    {
        public PostgresMiscellaneousTests()
            : base(new PostgresDBTestProvider())
        {
        }
    }
}
