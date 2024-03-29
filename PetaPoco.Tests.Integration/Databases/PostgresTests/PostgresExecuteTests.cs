﻿using PetaPoco.Tests.Integration.Providers;
using Xunit;

namespace PetaPoco.Tests.Integration.Databases.Postgres
{
    [Collection("Postgres")]
    public class PostgresExecuteTests : ExecuteTests
    {
        public PostgresExecuteTests()
            : base(new PostgresTestProvider())
        {
        }
    }
}
