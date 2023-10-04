using Xunit;

namespace PetaPoco.Tests.Integration.Databases.Sqlite
{
    [Collection("Sqlite")]
    public class SqliteExecuteTests : BaseExecuteTests
    {
        public SqliteExecuteTests()
            : base(new SqliteDBTestProvider())
        {
        }
    }
}