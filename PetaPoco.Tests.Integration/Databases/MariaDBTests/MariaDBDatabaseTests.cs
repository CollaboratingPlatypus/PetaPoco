using Xunit;

namespace PetaPoco.Tests.Integration.Databases.MariaDB
{
    [Collection("MariaDB")]
    public class MariaDBDatabaseTests : DatabaseTests
    {
        public MariaDBDatabaseTests()
            : base(new MariaDBDbProviderFactory())
        {
        }
    }
}
