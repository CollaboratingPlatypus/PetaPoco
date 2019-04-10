using System;
using System.Data.SqlClient;
using Xunit;

namespace PetaPoco.Tests.Integration.Databases.MSSQL
{
    [Collection("Mssql")]
    public class MssqlStoredProcTests : BaseStoredProcTests
    {
        protected override Type DataParameterType => typeof(SqlParameter);

        public MssqlStoredProcTests()
            : base(new MssqlDBTestProvider())
        {
        }
    }
}