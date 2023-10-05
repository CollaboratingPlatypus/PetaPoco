using Xunit;

namespace PetaPoco.Tests.Integration.Databases.MySqlConnector
{
    [Collection("MySqlConnector")]
    public class MySqlConnectorUpdateTests : UpdateTests
    {
        public MySqlConnectorUpdateTests()
            : base(new MySqlConnectorDbProviderFactory())
        {
        }
    }
}
