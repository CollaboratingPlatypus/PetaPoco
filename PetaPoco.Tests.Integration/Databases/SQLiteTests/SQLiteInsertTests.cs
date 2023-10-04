using Xunit;

namespace PetaPoco.Tests.Integration.Databases.Sqlite
{
    [Collection("Sqlite")]
    public class SqliteInsertTests : BaseInsertTests
    {
        public SqliteInsertTests()
            : base(new SqliteDBTestProvider())
        {
        }
    }
}