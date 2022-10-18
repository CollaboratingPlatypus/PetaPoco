using Xunit;

namespace PetaPoco.Tests.Integration.Databases.MySqlConnector
{
    [Collection("MySqlConnector")]
    [Trait("Category", "MySqlConnector")]
    public class MySqlConnectorUpdateTests : BaseUpdateTests
    {
        public MySqlConnectorUpdateTests()
            : base(new MySqlConnectorDBTestProvider())
        {
        }
    }
}
