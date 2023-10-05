using Xunit;

namespace PetaPoco.Tests.Integration.Databases.MySqlConnector
{
    [Collection("MySqlConnector")]
    public class MySqlConnectorInsertTests : InsertTests
    {
        public MySqlConnectorInsertTests()
            : base(new MySqlConnectorDbProviderFactory())
        {
        }
    }
}
