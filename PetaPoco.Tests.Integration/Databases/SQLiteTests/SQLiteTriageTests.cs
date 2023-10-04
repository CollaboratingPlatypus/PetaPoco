using Xunit;

namespace PetaPoco.Tests.Integration.Databases.Sqlite
{
    [Collection("SQLite")]
    public class SqliteTriageTests : BaseTriageTests
    {
        public SqliteTriageTests()
            : base(new SqliteDBTestProvider())
        {
        }
    }
}
