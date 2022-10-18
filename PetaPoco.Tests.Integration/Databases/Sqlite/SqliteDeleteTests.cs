using Xunit;

namespace PetaPoco.Tests.Integration.Databases.Sqlite
{
    [Collection("Sqlite")]
    [Trait("Category", "Sqlite")]
    public class SqliteDeleteTests : BaseDeleteTests
    {
        public SqliteDeleteTests()
            : base(new SqliteDBTestProvider())
        {
        }
    }
}
