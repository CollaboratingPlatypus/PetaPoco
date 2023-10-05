using System;
using Microsoft.Data.SqlClient;
using Xunit;

namespace PetaPoco.Tests.Integration.Databases.SqlServerMSData
{
    [Collection("SqlServerMSData")]
    public class SqlServerMSDataStoredProcTests : StoredProcTests
    {
        protected override Type DataParameterType => typeof(SqlParameter);

        public SqlServerMSDataStoredProcTests()
            : base(new SqlServerMSDataDbProviderFactory())
        {
        }
    }
}
