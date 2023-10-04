using Xunit;

namespace PetaPoco.Tests.Integration.Databases.MariaDb
{
    [Collection("MariaDb")]
    public class MariaDbDeleteTests : BaseDeleteTests
    {
        public MariaDbDeleteTests()
            : base(new MariaDbDBTestProvider())
        {
        }
    }
}