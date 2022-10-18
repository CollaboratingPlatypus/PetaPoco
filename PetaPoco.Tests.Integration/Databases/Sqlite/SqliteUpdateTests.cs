using Xunit;

namespace PetaPoco.Tests.Integration.Databases.Sqlite
{
    [Collection("Sqlite")]
    [Trait("Category", "Sqlite")]
    public class SqliteUpdateTests : BaseUpdateTests
    {
        public SqliteUpdateTests()
            : base(new SqliteDBTestProvider())
        {
        }
    }
}
