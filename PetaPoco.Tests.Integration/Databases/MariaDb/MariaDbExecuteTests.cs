using Xunit;

namespace PetaPoco.Tests.Integration.Databases.MariaDb
{
    [Collection("MariaDb")]
    [Trait("Category", "MariaDb")]
    public class MariaDbExecuteTests : BaseExecuteTests
    {
        public MariaDbExecuteTests()
            : base(new MariaDbDBTestProvider())
        {
        }
    }
}
