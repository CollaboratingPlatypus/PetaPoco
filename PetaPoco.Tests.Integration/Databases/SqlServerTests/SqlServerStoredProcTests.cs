using System;
using System.Data.SqlClient;
using Xunit;

namespace PetaPoco.Tests.Integration.Databases.SqlServer
{
    [Collection("SqlServer")]
    public class SqlServerStoredProcTests : StoredProcTests
    {
        protected override Type DataParameterType => typeof(SqlParameter);

        public SqlServerStoredProcTests()
            : base(new SqlServerDbProviderFactory())
        {
        }
    }
}
