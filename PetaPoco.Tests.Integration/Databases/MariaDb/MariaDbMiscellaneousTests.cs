using Xunit;

namespace PetaPoco.Tests.Integration.Databases.MariaDb
{
    [Collection("MariaDb")]
    public class MariaDbMiscellaneousTests : BaseMiscellaneousTests
    {
        public MariaDbMiscellaneousTests()
            : base(new MariaDbDBTestProvider())
        {
        }
    }
}
