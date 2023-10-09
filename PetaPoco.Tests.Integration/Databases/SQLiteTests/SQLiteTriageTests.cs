using PetaPoco.Tests.Integration.Providers;
using Xunit;

namespace PetaPoco.Tests.Integration.Databases.SQLite
{
    [Collection("SQLite")]
    public class SQLiteTriageTests : TriageTests
    {
        public SQLiteTriageTests()
            : base(new SQLiteTestProvider())
        {
        }
    }
}
