using Xunit;

namespace PetaPoco.Tests.Integration.Databases.MariaDb
{
    [Collection("MariaDb")]
    [Trait("Category", "MariaDb")]
    public class MariaDbDatabaseTests : BaseDatabaseTests
    {
        public MariaDbDatabaseTests()
            : base(new MariaDbDBTestProvider())
        {
        }
    }
}
