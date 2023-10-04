using Xunit;

namespace PetaPoco.Tests.Integration.Databases.MariaDb
{
    [Collection("MariaDB")]
    public class MariaDbUpdateTests : BaseUpdateTests
    {
        public MariaDbUpdateTests()
            : base(new MariaDbDBTestProvider())
        {
        }
    }
}
