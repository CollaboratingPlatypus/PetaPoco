using Xunit;

namespace PetaPoco.Tests.Integration.Databases.Sqlite
{
    [Collection("Sqlite")]
    [Trait("Category", "Sqlite")]
    public class SqliteExecuteTests : BaseExecuteTests
    {
        public SqliteExecuteTests()
            : base(new SqliteDBTestProvider())
        {
        }
    }
}
