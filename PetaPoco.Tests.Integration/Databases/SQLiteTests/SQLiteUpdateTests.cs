using Xunit;

namespace PetaPoco.Tests.Integration.Databases.Sqlite
{
    [Collection("SQLite")]
    public class SqliteUpdateTests : BaseUpdateTests
    {
        public SqliteUpdateTests()
            : base(new SqliteDBTestProvider())
        {
        }
    }
}
