using Xunit;

namespace PetaPoco.Tests.Integration.Databases.SqlServer
{
    public abstract partial class SqlServerDatabaseTests : DatabaseTests
    {
        protected SqlServerDatabaseTests(BaseDbProviderFactory provider)
            : base(provider)
        { }

        [Collection("SqlServer.SystemData")]
        public class SystemData : SqlServerDatabaseTests
        {
            public SystemData()
                : base(new SqlServerSystemDataDbProviderFactory())
            {
            }
        }

        [Collection("SqlServer.MicrosoftData")]
        public class MicrosoftData : SqlServerDatabaseTests
        {
            public MicrosoftData()
                : base(new SqlServerMSDataDbProviderFactory())
            {
            }
        }
    }
}
