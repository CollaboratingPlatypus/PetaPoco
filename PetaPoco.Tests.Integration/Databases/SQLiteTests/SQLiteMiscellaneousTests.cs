using Xunit;

namespace PetaPoco.Tests.Integration.Databases.Sqlite
{
    [Collection("SQLite")]
    public class SqliteMiscellaneousTests : BaseMiscellaneousTests
    {
        public SqliteMiscellaneousTests()
            : base(new SqliteDBTestProvider())
        {
        }
    }
}
