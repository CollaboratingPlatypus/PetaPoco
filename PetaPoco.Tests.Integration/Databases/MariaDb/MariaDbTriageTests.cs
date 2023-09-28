using Xunit;

namespace PetaPoco.Tests.Integration.Databases.MariaDb
{
    [Collection("MariaDb")]
    public class MariaDbTriageTests : BaseTriageTests
    {
        public MariaDbTriageTests()
            : base(new MariaDbDBTestProvider())
        {
        }
    }
}
