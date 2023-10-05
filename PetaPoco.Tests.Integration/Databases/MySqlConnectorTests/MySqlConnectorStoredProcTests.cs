using System;
using MySqlConnector;
using Xunit;

namespace PetaPoco.Tests.Integration.Databases.MySqlConnector
{
    [Collection("MySqlConnector")]
    public class MySqlConnectorStoredProcTests : StoredProcTests
    {
        protected override Type DataParameterType => typeof(MySqlParameter);

        public MySqlConnectorStoredProcTests()
            : base(new MySqlConnectorDbProviderFactory())
        {
        }
    }
}
