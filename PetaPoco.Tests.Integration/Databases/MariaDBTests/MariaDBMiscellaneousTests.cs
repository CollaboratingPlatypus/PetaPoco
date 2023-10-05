using Xunit;

namespace PetaPoco.Tests.Integration.Databases.MariaDB
{
    [Collection("MariaDB")]
    public class MariaDBMiscellaneousTests : MiscellaneousTests
    {
        public MariaDBMiscellaneousTests()
            : base(new MariaDBDbProviderFactory())
        {
        }
    }
}
