using Xunit;

namespace PetaPoco.Tests.Integration.Databases.MySqlConnector
{
    [Collection("MySqlConnector")]
    public class MySqlConnectorExecuteTests : ExecuteTests
    {
        public MySqlConnectorExecuteTests()
            : base(new MySqlConnectorDbProviderFactory())
        {
        }
    }
}
