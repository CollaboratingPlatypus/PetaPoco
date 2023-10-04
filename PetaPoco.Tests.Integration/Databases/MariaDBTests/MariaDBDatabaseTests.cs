using Xunit;

namespace PetaPoco.Tests.Integration.Databases.MariaDb
{
    [Collection("MariaDB")]
    public class MariaDbDatabaseTests : BaseDatabaseTests
    {
        public MariaDbDatabaseTests()
            : base(new MariaDbDBTestProvider())
        {
        }
    }
}
