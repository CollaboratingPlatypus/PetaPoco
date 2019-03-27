using Xunit;

namespace PetaPoco.Tests.Integration.Databases.Sqlite
{
    [Collection("Sqlite")]
    public class SqliteUpdateTests : BaseUpdateTests
    {
        public SqliteUpdateTests()
            : base(new SqliteDBTestProvider())
        {
        }
    }
}