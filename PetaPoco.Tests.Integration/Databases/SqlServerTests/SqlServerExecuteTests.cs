using PetaPoco.Tests.Integration.Providers;
using Xunit;

namespace PetaPoco.Tests.Integration.Databases.SqlServer
{
    public abstract partial class SqlServerExecuteTests : ExecuteTests
    {
        protected SqlServerExecuteTests(TestProvider provider)
            : base(provider)
        {
        }

        [Collection("SqlServer.SystemData")]
        public class SystemData : SqlServerExecuteTests
        {
            public SystemData()
                : base(new SqlServerSystemDataTestProvider())
            {
            }
        }

        [Collection("SqlServer.MicrosoftData")]
        public class MicrosoftData : SqlServerExecuteTests
        {
            public MicrosoftData()
                : base(new SqlServerMSDataTestProvider())
            {
            }
        }
    }
}
