using Xunit;

namespace PetaPoco.Tests.Integration.Databases.MySqlConnector
{
    [Collection("MySqlConnector")]
    public class MySqlConnectorMiscellaneousTests : MiscellaneousTests
    {
        public MySqlConnectorMiscellaneousTests()
            : base(new MySqlConnectorDbProviderFactory())
        {
        }
    }
}
