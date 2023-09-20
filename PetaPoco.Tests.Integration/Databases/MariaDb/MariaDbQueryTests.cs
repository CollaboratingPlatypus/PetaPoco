using Xunit;

namespace PetaPoco.Tests.Integration.Databases.MariaDb
{
    [Collection("MariaDb")]
    public class MariaDbQueryTests : BaseQueryTests
    {
        public MariaDbQueryTests()
            : base(new MariaDbDBTestProvider())
        {
        }
    }
}
