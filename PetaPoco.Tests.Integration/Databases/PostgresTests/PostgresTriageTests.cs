using Xunit;

namespace PetaPoco.Tests.Integration.Databases.Postgres
{
    [Collection("Postgres")]
    public class PostgresTriageTests : TriageTests
    {
        public PostgresTriageTests()
            : base(new PostgresDbProviderFactory())
        {
        }
    }
}
