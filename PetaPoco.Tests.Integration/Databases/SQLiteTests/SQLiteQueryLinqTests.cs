using Xunit;

namespace PetaPoco.Tests.Integration.Databases.SQLite
{
    [Collection("SQLite")]
    public class SQLiteQueryLinqTests : QueryLinqTests
    {
        public SQLiteQueryLinqTests()
            : base(new SQLiteDbProviderFactory())
        {
        }
    }
}
