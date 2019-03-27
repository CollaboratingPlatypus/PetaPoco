using Xunit;

namespace PetaPoco.Tests.Integration.Databases.Sqlite
{
    [Collection("Sqlite")]
    public class SqliteDeleteTests : BaseDeleteTests
    {
        public SqliteDeleteTests()
            : base(new SqliteDBTestProvider())
        {
        }
    }
}