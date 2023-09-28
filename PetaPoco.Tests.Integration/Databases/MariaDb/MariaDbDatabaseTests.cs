using Xunit;

namespace PetaPoco.Tests.Integration.Databases.MariaDb
{
    [Collection("MariaDb")]
    public class MariaDbDatabaseTests : BaseDatabaseTests
    {
        public MariaDbDatabaseTests()
            : base(new MariaDbDBTestProvider())
        {
        }
    }
}
