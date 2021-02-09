using Xunit;

namespace PetaPoco.Tests.Integration.Databases.MySqlConnector
{
    [Collection("MySqlConnector")]
    public class MySqlConnectorQueryLinqTests : BaseQueryLinqTests
    {
        public MySqlConnectorQueryLinqTests()
            : base(new MySqlConnectorDBTestProvider())
        {
        }
    }
}