using Xunit;

namespace PetaPoco.Tests.Integration.Databases.MySqlConnector
{
    [Collection("MySqlConnector")]
    public class MySqlConnectorMiscellaneousTests : BaseMiscellaneousTests
    {
        public MySqlConnectorMiscellaneousTests()
            : base(new MySqlConnectorDBTestProvider())
        {
        }
    }
}
