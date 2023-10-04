using Xunit;

namespace PetaPoco.Tests.Integration.Databases.MariaDb
{
    [Collection("MariaDB")]
    public class MariaDbExecuteTests : BaseExecuteTests
    {
        public MariaDbExecuteTests()
            : base(new MariaDbDBTestProvider())
        {
        }
    }
}
