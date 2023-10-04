using Xunit;

namespace PetaPoco.Tests.Integration.Databases.MariaDb
{
    [Collection("MariaDB")]
    public class MariaDbQueryLinqTests : BaseQueryLinqTests
    {
        public MariaDbQueryLinqTests()
            : base(new MariaDbDBTestProvider())
        {
        }
    }
}
