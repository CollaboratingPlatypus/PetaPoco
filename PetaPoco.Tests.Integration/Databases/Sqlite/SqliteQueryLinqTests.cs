using Xunit;

namespace PetaPoco.Tests.Integration.Databases.Sqlite
{
    [Collection("Sqlite")]
    public class SqliteQueryLinqTests : BaseQueryLinqTests
    {
        public SqliteQueryLinqTests()
            : base(new SqliteDBTestProvider())
        {
        }
    }
}