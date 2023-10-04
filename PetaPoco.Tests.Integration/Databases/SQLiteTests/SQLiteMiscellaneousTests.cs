using Xunit;

namespace PetaPoco.Tests.Integration.Databases.Sqlite
{
    [Collection("Sqlite")]
    public class SqliteMiscellaneousTests : BaseMiscellaneousTests
    {
        public SqliteMiscellaneousTests()
            : base(new SqliteDBTestProvider())
        {
        }
    }
}
