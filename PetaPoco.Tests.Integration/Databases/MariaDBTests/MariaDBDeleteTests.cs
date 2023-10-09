using PetaPoco.Tests.Integration.Providers;
using Xunit;

namespace PetaPoco.Tests.Integration.Databases.MariaDB
{
    [Collection("MariaDB")]
    public class MariaDBDeleteTests : DeleteTests
    {
        public MariaDBDeleteTests()
            : base(new MariaDBTestProvider())
        {
        }
    }
}
