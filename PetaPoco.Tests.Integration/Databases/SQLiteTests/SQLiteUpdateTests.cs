using PetaPoco.Tests.Integration.Providers;
using Xunit;

namespace PetaPoco.Tests.Integration.Databases.SQLite
{
    [Collection("SQLite")]
    public class SQLiteUpdateTests : UpdateTests
    {
        public SQLiteUpdateTests()
            : base(new SQLiteTestProvider())
        {
        }
    }
}
