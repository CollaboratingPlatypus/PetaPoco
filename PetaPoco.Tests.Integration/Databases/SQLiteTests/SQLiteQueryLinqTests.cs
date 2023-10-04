using Xunit;

namespace PetaPoco.Tests.Integration.Databases.Sqlite
{
    [Collection("SQLite")]
    public class SqliteQueryLinqTests : BaseQueryLinqTests
    {
        public SqliteQueryLinqTests()
            : base(new SqliteDBTestProvider())
        {
        }
    }
}
