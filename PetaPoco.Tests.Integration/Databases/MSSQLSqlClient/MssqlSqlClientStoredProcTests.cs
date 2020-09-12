using System;
using Microsoft.Data.SqlClient;
using Xunit;

namespace PetaPoco.Tests.Integration.Databases.MSSQLSqlClient
{
    [Collection("MssqlSqlClient")]
    public class MssqlSqlClientStoredProcTests : BaseStoredProcTests
    {
        protected override Type DataParameterType => typeof(SqlParameter);

        public MssqlSqlClientStoredProcTests()
            : base(new MssqlSqlClientDBTestProvider())
        {
        }
    }
}