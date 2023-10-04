using Xunit;

namespace PetaPoco.Tests.Integration.Databases.Sqlite
{
    [Collection("SQLite")]
    public class SqliteExecuteTests : BaseExecuteTests
    {
        public SqliteExecuteTests()
            : base(new SqliteDBTestProvider())
        {
        }
    }
}
