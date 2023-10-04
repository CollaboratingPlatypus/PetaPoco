using Xunit;

namespace PetaPoco.Tests.Integration.Databases.MySqlConnector
{
    [Collection("MySqlConnector")]
    public class MySqlConnectorDeleteTests : BaseDeleteTests
    {
        public MySqlConnectorDeleteTests()
            : base(new MySqlConnectorDBTestProvider())
        {
        }
    }
}