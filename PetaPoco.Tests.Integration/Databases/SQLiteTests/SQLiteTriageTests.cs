using Xunit;

namespace PetaPoco.Tests.Integration.Databases.Sqlite
{
    [Collection("Sqlite")]
    public class SqliteTriageTests : BaseTriageTests
    {
        public SqliteTriageTests()
            : base(new SqliteDBTestProvider())
        {
        }
    }
}
