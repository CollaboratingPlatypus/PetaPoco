using System;
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
    }
}
