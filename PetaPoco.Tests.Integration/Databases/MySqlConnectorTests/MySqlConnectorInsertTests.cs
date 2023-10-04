using Xunit;

namespace PetaPoco.Tests.Integration.Databases.MySqlConnector
{
    [Collection("MySqlConnector")]
    public class MySqlConnectorInsertTests : BaseInsertTests
    {
        public MySqlConnectorInsertTests()
            : base(new MySqlConnectorDBTestProvider())
        {
        }
    }
}