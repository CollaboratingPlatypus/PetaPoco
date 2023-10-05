using Xunit;

namespace PetaPoco.Tests.Integration.Databases.SQLite
{
    [Collection("SQLite")]
    public class SQLiteDeleteTests : DeleteTests
    {
        public SQLiteDeleteTests()
            : base(new SQLiteDbProviderFactory())
        {
        }
    }
}
