﻿using Xunit;

namespace PetaPoco.Tests.Integration.Databases.MySqlConnector
{
    [Collection("MySqlConnector")]
    [Trait("Category", "MySqlConnector")]
    public class MySqlConnectorQueryTests : BaseQueryTests
    {
        public MySqlConnectorQueryTests()
            : base(new MySqlConnectorDBTestProvider())
        {
        }
    }
}
