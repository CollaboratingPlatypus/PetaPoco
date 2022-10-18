using Xunit;

namespace PetaPoco.Tests.Integration.Databases.MySqlConnector
{
    [Collection("MySqlConnector")]
    [Trait("Category", "MySqlConnector")]
    public class MySqlConnectorExecuteTests : BaseExecuteTests
    {
        public MySqlConnectorExecuteTests()
            : base(new MySqlConnectorDBTestProvider())
        {
        }
    }
}
