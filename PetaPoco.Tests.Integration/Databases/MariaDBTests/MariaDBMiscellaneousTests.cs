using Xunit;

namespace PetaPoco.Tests.Integration.Databases.MariaDb
{
    [Collection("MariaDB")]
    public class MariaDbMiscellaneousTests : BaseMiscellaneousTests
    {
        public MariaDbMiscellaneousTests()
            : base(new MariaDbDBTestProvider())
        {
        }
    }
}
