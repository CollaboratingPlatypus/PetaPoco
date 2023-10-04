using Xunit;

namespace PetaPoco.Tests.Integration.Databases.MySqlConnector
{
    [Collection("MySqlConnector")]
    public class MySqlConnectorTriageTests : BaseTriageTests
    {
        public MySqlConnectorTriageTests()
            : base(new MySqlConnectorDBTestProvider())
        {
        }
    }
}
