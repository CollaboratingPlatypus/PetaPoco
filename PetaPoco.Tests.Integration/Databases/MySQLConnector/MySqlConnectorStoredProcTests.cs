using System;
using MySqlConnector;
using Xunit;

namespace PetaPoco.Tests.Integration.Databases.MySqlConnector
{
    [Collection("MySqlConnector")]
    [Trait("Category", "MySqlConnector")]
    public class MySqlConnectorStoredProcTests : BaseStoredProcTests
    {
        protected override Type DataParameterType => typeof(MySqlParameter);

        public MySqlConnectorStoredProcTests()
            : base(new MySqlConnectorDBTestProvider())
        {
        }
    }
}
