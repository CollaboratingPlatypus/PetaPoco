using Xunit;

namespace PetaPoco.Tests.Integration.Databases.SqlServer
{
    public abstract partial class SqlServerExecuteTests : ExecuteTests
    {
        protected SqlServerExecuteTests(BaseDbProviderFactory provider)
            : base(provider)
        { }

        [Collection("SqlServer.SystemData")]
        public class SystemData : SqlServerExecuteTests
        {
            public SystemData()
                : base(new SqlServerSystemDataDbProviderFactory())
            {
            }
        }

        [Collection("SqlServer.MicrosoftData")]
        public class MicrosoftData : SqlServerExecuteTests
        {
            public MicrosoftData()
                : base(new SqlServerMSDataDbProviderFactory())
            {
            }
        }
    }
}
