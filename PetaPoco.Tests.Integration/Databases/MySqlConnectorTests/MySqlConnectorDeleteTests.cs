using PetaPoco.Tests.Integration.Providers;
using Xunit;

namespace PetaPoco.Tests.Integration.Databases.MySqlConnector
{
    [Collection("MySqlConnector")]
    public class MySqlConnectorDeleteTests : DeleteTests
    {
        public MySqlConnectorDeleteTests()
            : base(new MySqlConnectorTestProvider())
        {
        }
    }
}
