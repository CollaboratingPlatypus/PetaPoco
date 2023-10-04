using Xunit;

namespace PetaPoco.Tests.Integration.Databases.Sqlite
{
    [Collection("SQLite")]
    public class SqliteDeleteTests : BaseDeleteTests
    {
        public SqliteDeleteTests()
            : base(new SqliteDBTestProvider())
        {
        }
    }
}
