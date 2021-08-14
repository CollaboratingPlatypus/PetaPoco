using System;
using Npgsql;
using Xunit;

namespace PetaPoco.Tests.Integration.Databases.Postgres
{
    [Collection("Postgres")]
    [Trait("Category", "Postgres")]
    public class PostgresStoredProcTests : BaseStoredProcTests
    {
        protected override Type DataParameterType => typeof(NpgsqlParameter);

        public PostgresStoredProcTests()
            : base(new PostgresDBTestProvider())
        {
        }
    }
}