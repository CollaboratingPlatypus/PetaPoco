using Xunit;

namespace PetaPoco.Tests.Integration.Databases.SqlServer
{
    public abstract partial class SqlServerDeleteTests : DeleteTests
    {
        protected SqlServerDeleteTests(BaseDbProviderFactory provider)
            : base(provider)
        { }

        [Collection("SqlServer.SystemData")]
        public class SystemData : SqlServerDeleteTests
        {
            public SystemData()
                : base(new SqlServerSystemDataDbProviderFactory())
            {
            }
        }

        [Collection("SqlServer.MicrosoftData")]
        public class MicrosoftData : SqlServerDeleteTests
        {
            public MicrosoftData()
                : base(new SqlServerMSDataDbProviderFactory())
            {
            }
        }
    }
}
