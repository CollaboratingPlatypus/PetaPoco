using Xunit;

namespace PetaPoco.Tests.Integration.Databases.Postgres
{
    [Collection("Postgres")]
    public class PostgresTriageTests : BaseTriageTests
    {
        public PostgresTriageTests()
            : base(new PostgresDBTestProvider())
        {
        }
    }
}
