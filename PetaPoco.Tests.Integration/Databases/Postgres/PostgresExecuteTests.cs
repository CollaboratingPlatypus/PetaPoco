using Xunit;

namespace PetaPoco.Tests.Integration.Databases.Postgres
{
    [Collection("Postgres")]
    public class PostgresExecuteTests : BaseExecuteTests
    {
        public PostgresExecuteTests()
            : base(new PostgresDBTestProvider())
        {
        }
    }
}