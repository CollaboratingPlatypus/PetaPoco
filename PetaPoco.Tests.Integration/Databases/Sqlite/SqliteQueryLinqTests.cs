using Xunit;

namespace PetaPoco.Tests.Integration.Databases.Sqlite
{
    [Collection("Sqlite")]
    [Trait("Category", "Sqlite")]
    public class SqliteQueryLinqTests : BaseQueryLinqTests
    {
        public SqliteQueryLinqTests()
            : base(new SqliteDBTestProvider())
        {
        }
    }
}
