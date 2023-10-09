using PetaPoco.Tests.Integration.Providers;
using Xunit;

namespace PetaPoco.Tests.Integration.Databases.MariaDB
{
    [Collection("MariaDB")]
    public class MariaDBUpdateTests : UpdateTests
    {
        public MariaDBUpdateTests()
            : base(new MariaDBTestProvider())
        {
        }
    }
}
