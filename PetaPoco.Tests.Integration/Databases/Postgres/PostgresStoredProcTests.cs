using Npgsql;
using System;
// <copyright company="PetaPoco - CollaboratingPlatypus">
//      Apache License, Version 2.0 https://github.com/CollaboratingPlatypus/PetaPoco/blob/master/LICENSE.txt
// </copyright>
// <author>PetaPoco - CollaboratingPlatypus</author>
// <date>2018/07/02</date>

using System;
using Xunit;

namespace PetaPoco.Tests.Integration.Databases.Postgres
{
    [Collection("Postgres")]
    public class PostgresStoredProcTests : BaseStoredProcTests
    {
        public PostgresStoredProcTests()
            : base(new PostgresDBTestProvider())
        {
        }

        protected override Type DataParameterType => typeof(NpgsqlParameter);
    }
}
