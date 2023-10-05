using Xunit;

namespace PetaPoco.Tests.Integration.Databases.SQLite
{
    [Collection("SQLite")]
    public class SQLiteExecuteTests : ExecuteTests
    {
        public SQLiteExecuteTests()
            : base(new SQLiteDbProviderFactory())
        {
        }
    }
}
