using PetaPoco.Tests.Integration.Providers;
using Xunit;

namespace PetaPoco.Tests.Integration.Databases.MySqlConnector
{
    [Collection("MySqlConnector")]
    public class MySqlConnectorTriageTests : TriageTests
    {
        public MySqlConnectorTriageTests()
            : base(new MySqlConnectorTestProvider())
        {
        }
    }
}
