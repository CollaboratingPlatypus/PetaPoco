using Xunit;

namespace PetaPoco.Tests.Integration.Databases.Sqlite
{
    [Collection("Sqlite")]
    [Trait("Category", "Sqlite")]
    public class SqliteQueryTests : BaseQueryTests
    {
        public SqliteQueryTests()
            : base(new SqliteDBTestProvider())
        {
        }
    }
}
