using Xunit;

namespace PetaPoco.Tests.Integration.Databases.MariaDb
{
    [Collection("MariaDb")]
    [Trait("Category", "MariaDb")]
    public class MariaDbUpdateTests : BaseUpdateTests
    {
        public MariaDbUpdateTests()
            : base(new MariaDbDBTestProvider())
        {
        }
    }
}
