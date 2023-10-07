using System;
using Xunit;

namespace PetaPoco.Tests.Integration.Databases.SqlServer
{
    public abstract partial class SqlServerStoredProcTests : StoredProcTests
    {
        protected SqlServerStoredProcTests(BaseDbProviderFactory provider)
            : base(provider)
        { }

        [Collection("SqlServer.SystemData")]
        public class SystemData : SqlServerStoredProcTests
        {
            protected override Type DataParameterType => typeof(System.Data.SqlClient.SqlParameter);

            public SystemData()
                : base(new SqlServerSystemDataDbProviderFactory())
            {
            }
        }

        [Collection("SqlServer.MicrosoftData")]
        public class MicrosoftData : SqlServerStoredProcTests
        {
            protected override Type DataParameterType => typeof(Microsoft.Data.SqlClient.SqlParameter);

            public MicrosoftData()
                : base(new SqlServerMSDataDbProviderFactory())
            {
            }
        }
    }
}
