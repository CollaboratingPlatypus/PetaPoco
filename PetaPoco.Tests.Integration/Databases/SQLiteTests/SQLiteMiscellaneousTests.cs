using Xunit;

namespace PetaPoco.Tests.Integration.Databases.SQLite
{
    [Collection("SQLite")]
    public class SQLiteMiscellaneousTests : MiscellaneousTests
    {
        public SQLiteMiscellaneousTests()
            : base(new SQLiteDbProviderFactory())
        {
        }
    }
}
