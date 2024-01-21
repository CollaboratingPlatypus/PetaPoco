using PetaPoco.Tests.Integration.Providers;
using Xunit;

namespace PetaPoco.Tests.Integration.Databases.Postgres
{
    [Collection("Postgres")]
    public class PostgresMiscellaneousTests : MiscellaneousTests
    {
        public PostgresMiscellaneousTests()
            : base(new PostgresTestProvider())
        {
        }
    }
}
