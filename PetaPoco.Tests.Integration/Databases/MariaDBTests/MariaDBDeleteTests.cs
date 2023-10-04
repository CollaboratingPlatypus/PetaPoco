using Xunit;

namespace PetaPoco.Tests.Integration.Databases.MariaDb
{
    [Collection("MariaDB")]
    public class MariaDbDeleteTests : BaseDeleteTests
    {
        public MariaDbDeleteTests()
            : base(new MariaDbDBTestProvider())
        {
        }
    }
}
