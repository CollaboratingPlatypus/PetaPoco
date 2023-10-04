using Xunit;

namespace PetaPoco.Tests.Integration.Databases.Sqlite
{
    [Collection("SQLite")]
    public class SqliteInsertTests : BaseInsertTests
    {
        public SqliteInsertTests()
            : base(new SqliteDBTestProvider())
        {
        }
    }
}
