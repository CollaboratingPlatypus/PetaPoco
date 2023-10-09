using PetaPoco.Tests.Integration.Providers;
using Xunit;

namespace PetaPoco.Tests.Integration.Databases.MariaDB
{
    [Collection("MariaDB")]
    public class MariaDBTriageTests : TriageTests
    {
        public MariaDBTriageTests()
            : base(new MariaDBTestProvider())
        {
        }
    }
}
