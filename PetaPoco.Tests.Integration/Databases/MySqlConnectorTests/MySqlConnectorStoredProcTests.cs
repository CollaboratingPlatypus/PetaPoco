using System;
using MySqlConnector;
using PetaPoco.Tests.Integration.Providers;
using Xunit;

namespace PetaPoco.Tests.Integration.Databases.MySqlConnector
{
    [Collection("MySqlConnector")]
    public class MySqlConnectorStoredProcTests : StoredProcTests
    {
        protected override Type DataParameterType => typeof(MySqlParameter);

        public MySqlConnectorStoredProcTests()
            : base(new MySqlConnectorTestProvider())
        {
        }
    }
}
