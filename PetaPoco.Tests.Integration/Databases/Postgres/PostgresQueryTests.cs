using Xunit;

namespace PetaPoco.Tests.Integration.Databases.Postgres
{
    [Collection("Postgres")]
    public class PostgresQueryTests : BaseQueryTests
    {
        public PostgresQueryTests()
            : base(new PostgresDBTestProvider())
        {
        }
    }
}