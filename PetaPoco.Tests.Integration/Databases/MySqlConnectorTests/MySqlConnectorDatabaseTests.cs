using PetaPoco.Tests.Integration.Providers;
using Xunit;

namespace PetaPoco.Tests.Integration.Databases.MySqlConnector
{
    [Collection("MySqlConnector")]
    public class MySqlConnectorDatabaseTests : DatabaseTests
    {
        public MySqlConnectorDatabaseTests()
            : base(new MySqlConnectorTestProvider())
        {
        }
    }
}
