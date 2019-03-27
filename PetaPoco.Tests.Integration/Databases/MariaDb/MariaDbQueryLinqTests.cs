using Xunit;

namespace PetaPoco.Tests.Integration.Databases.MariaDb
{
    [Collection("MariaDb")]
    public class MariaDbQueryLinqTests : BaseQueryLinqTests
    {
        public MariaDbQueryLinqTests()
            : base(new MariaDbDBTestProvider())
        {
        }
    }
}