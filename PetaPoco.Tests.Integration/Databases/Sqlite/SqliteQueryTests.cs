using Xunit;

namespace PetaPoco.Tests.Integration.Databases.Sqlite
{
    [Collection("Sqlite")]
    public class SqliteQueryTests : BaseQueryTests
    {
        public SqliteQueryTests()
            : base(new SqliteDBTestProvider())
        {
        }
    }
}