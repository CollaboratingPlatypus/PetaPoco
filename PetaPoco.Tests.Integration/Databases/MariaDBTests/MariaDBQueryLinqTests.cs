using PetaPoco.Tests.Integration.Providers;
using Xunit;

namespace PetaPoco.Tests.Integration.Databases.MariaDB
{
    [Collection("MariaDB")]
    public class MariaDBQueryLinqTests : QueryLinqTests
    {
        public MariaDBQueryLinqTests()
            : base(new MariaDBTestProvider())
        {
        }
    }
}
