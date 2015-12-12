using Xunit;

namespace PetaPoco.Tests.Integration.Databases.Sqlite
{
    [Collection("SqliteTests")]
    public class SqliteUpdateTests : BaseUpdateTests
    {
        public SqliteUpdateTests()
            : base(new SqliteDBTestProvider())
        {
        }
    }
}