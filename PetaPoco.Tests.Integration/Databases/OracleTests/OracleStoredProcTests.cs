using System;
using Oracle.ManagedDataAccess.Client;
using PetaPoco.Tests.Integration.Providers;
using Xunit;

namespace PetaPoco.Tests.Integration.Databases.Oracle
{
    [Collection("Oracle")]
    public class OracleStoredProcTests : StoredProcTests
    {
        protected override Type DataParameterType => typeof(OracleParameter);

        public OracleStoredProcTests()
            : base(new OracleTestProvider())
        {
        }
    }
}
