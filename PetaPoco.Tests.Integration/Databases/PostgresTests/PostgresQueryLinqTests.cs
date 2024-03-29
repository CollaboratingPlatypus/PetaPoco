﻿using PetaPoco.Tests.Integration.Providers;
using Xunit;

namespace PetaPoco.Tests.Integration.Databases.Postgres
{
    [Collection("Postgres")]
    public class PostgresQueryLinqTests : QueryLinqTests
    {
        public PostgresQueryLinqTests()
            : base(new PostgresTestProvider())
        {
        }
    }
}
