using Xunit;

namespace PetaPoco.Tests.Integration.Databases.MariaDb
{
    [Collection("MariaDb")]
    public class MariaDbExecuteTests : BaseExecuteTests
    {
        public MariaDbExecuteTests()
            : base(new MariaDbDBTestProvider())
        {
        }
    }
}