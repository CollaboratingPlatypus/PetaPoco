using Xunit;

namespace PetaPoco.Tests.Integration.Databases.MySqlConnector
{
    [Collection("MySqlConnector")]
    public class MySqlConnectorUpdateTests : BaseUpdateTests
    {
        public MySqlConnectorUpdateTests()
            : base(new MySqlConnectorDBTestProvider())
        {
        }
    }
}