using System;
using Npgsql;
using PetaPoco.Tests.Integration.Providers;
using Xunit;

namespace PetaPoco.Tests.Integration.Databases.Postgres
{
    [Collection("Postgres")]
    public class PostgresStoredProcTests : StoredProcTests
    {
        protected override Type DataParameterType => typeof(NpgsqlParameter);

        public PostgresStoredProcTests()
            : base(new PostgresTestProvider())
        {
        }
    }
}
