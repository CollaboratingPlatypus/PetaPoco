using Xunit;

namespace PetaPoco.Tests.Integration.Databases.MariaDb
{
    [Collection("MariaDB")]
    public class MariaDbTriageTests : BaseTriageTests
    {
        public MariaDbTriageTests()
            : base(new MariaDbDBTestProvider())
        {
        }
    }
}
