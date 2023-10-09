using PetaPoco.Tests.Integration.Providers;
using Xunit;

namespace PetaPoco.Tests.Integration.Databases.MySqlConnector
{
    [Collection("MySqlConnector")]
    public class MySqlConnectorInsertTests : InsertTests
    {
        public MySqlConnectorInsertTests()
            : base(new MySqlConnectorTestProvider())
        {
        }
    }
}
