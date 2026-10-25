using System;
using System.Threading.Tasks;
using FirebirdSql.Data.FirebirdClient;
using PetaPoco.Tests.Integration.Providers;
using Xunit;

namespace PetaPoco.Tests.Integration.Databases.Firebird
{
    [Collection("Firebird")]
    public class FirebirdStoredProcTests : StoredProcTests
    {
        protected override Type DataParameterType => typeof(FbParameter);

        public FirebirdStoredProcTests()
            : base(new FirebirdTestProvider())
        {
        }

        [Fact(Skip = "Schema variant stored procedures not supported by provider")]
        public override async Task SchemaVariantProc_WithParam_ShouldReturnCorrectDynamicTypeCollection() { }
    }
}
