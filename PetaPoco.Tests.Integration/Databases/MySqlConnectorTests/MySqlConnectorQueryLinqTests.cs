using Xunit;

namespace PetaPoco.Tests.Integration.Databases.MySqlConnector
{
    [Collection("MySqlConnector")]
    public class MySqlConnectorQueryLinqTests : QueryLinqTests
    {
        public MySqlConnectorQueryLinqTests()
            : base(new MySqlConnectorDbProviderFactory())
        {
        }
    }
}
