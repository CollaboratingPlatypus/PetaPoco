using Xunit;

namespace PetaPoco.Tests.Integration.Databases.MySqlConnector
{
    [Collection("MySqlConnector")]
    [Trait("Category", "MySqlConnector")]
    public class MySqlConnectorQueryLinqTests : BaseQueryLinqTests
    {
        public MySqlConnectorQueryLinqTests()
            : base(new MySqlConnectorDBTestProvider())
        {
        }
    }
}
