using Xunit;

namespace PetaPoco.Tests.Integration.Databases.MariaDb
{
    [Collection("MariaDb")]
    [Trait("Category", "MariaDb")]
    public class MariaDbDeleteTests : BaseDeleteTests
    {
        public MariaDbDeleteTests()
            : base(new MariaDbDBTestProvider())
        {
        }
    }
}
