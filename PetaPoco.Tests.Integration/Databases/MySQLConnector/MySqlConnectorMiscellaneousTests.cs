using Xunit;

namespace PetaPoco.Tests.Integration.Databases.MySqlConnector
{
    [Collection("MySqlConnector")]
    public class MySqlConnectorMiscellaneousTests : BaseDatabase
    {
        public MySqlConnectorMiscellaneousTests()
            : base(new MySqlConnectorDBTestProvider())
        {
        }
    }
}
