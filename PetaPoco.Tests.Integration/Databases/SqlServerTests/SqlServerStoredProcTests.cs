using System;
using PetaPoco.Tests.Integration.Providers;
using Xunit;

namespace PetaPoco.Tests.Integration.Databases.SqlServer
{
    public abstract partial class SqlServerStoredProcTests : StoredProcTests
    {
        protected SqlServerStoredProcTests(TestProvider provider)
            : base(provider)
        {
        }

        [Collection("SqlServer.SystemData")]
        public class SystemData : SqlServerStoredProcTests
        {
            protected override Type DataParameterType => typeof(System.Data.SqlClient.SqlParameter);

            public SystemData()
                : base(new SqlServerSystemDataTestProvider())
            {
            }
        }

        [Collection("SqlServer.MicrosoftData")]
        public class MicrosoftData : SqlServerStoredProcTests
        {
            protected override Type DataParameterType => typeof(Microsoft.Data.SqlClient.SqlParameter);

            public MicrosoftData()
                : base(new SqlServerMSDataTestProvider())
            {
            }
        }
    }
}
