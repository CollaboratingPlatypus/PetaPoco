using Xunit;

namespace PetaPoco.Tests.Integration.Databases.MariaDb
{
    [Collection("MariaDb")]
    public class MariaQueryTests : BaseQueryTests
    {
        public MariaQueryTests()
            : base(new MariaDbDBTestProvider())
        {
        }
    }
}