using Xunit;

namespace PetaPoco.Tests.Integration.Databases.Sqlite
{
    [Collection("Sqlite")]
    public class SqliteMiscellaneousTests : BaseDatabase
    {
        public SqliteMiscellaneousTests()
            : base(new SqliteDBTestProvider())
        {
        }
    }
}
