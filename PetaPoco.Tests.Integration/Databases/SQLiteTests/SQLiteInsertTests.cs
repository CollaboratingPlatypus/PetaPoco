using Xunit;

namespace PetaPoco.Tests.Integration.Databases.SQLite
{
    [Collection("SQLite")]
    public class SQLiteInsertTests : InsertTests
    {
        public SQLiteInsertTests()
            : base(new SQLiteDbProviderFactory())
        {
        }
    }
}
