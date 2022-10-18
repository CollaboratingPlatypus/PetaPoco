using Xunit;

namespace PetaPoco.Tests.Integration.Databases.Sqlite
{
    [Collection("Sqlite")]
    [Trait("Category", "Sqlite")]
    public class SqliteInsertTests : BaseInsertTests
    {
        public SqliteInsertTests()
            : base(new SqliteDBTestProvider())
        {
        }
    }
}
