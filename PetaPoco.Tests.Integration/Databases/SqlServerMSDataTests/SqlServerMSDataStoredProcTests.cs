using System;
using Microsoft.Data.SqlClient;
using Xunit;

namespace PetaPoco.Tests.Integration.Databases.MSSQLMsData
{
    [Collection("MssqlMsData")]
    public class MssqlMsDataStoredProcTests : BaseStoredProcTests
    {
        protected override Type DataParameterType => typeof(SqlParameter);

        public MssqlMsDataStoredProcTests()
            : base(new MssqlMsDataDBTestProvider())
        {
        }
    }
}