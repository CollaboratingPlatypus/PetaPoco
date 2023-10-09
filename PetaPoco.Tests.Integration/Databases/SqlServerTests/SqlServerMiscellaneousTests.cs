using PetaPoco.Tests.Integration.Providers;
using Xunit;

namespace PetaPoco.Tests.Integration.Databases.SqlServer
{
    public abstract partial class SqlServerMiscellaneousTests : MiscellaneousTests
    {
        protected SqlServerMiscellaneousTests(TestProvider provider)
            : base(provider)
        {
        }

        [Collection("SqlServer.SystemData")]
        public class SystemData : SqlServerMiscellaneousTests
        {
            public SystemData()
                : base(new SqlServerSystemDataTestProvider())
            {
            }
        }

        [Collection("SqlServer.MicrosoftData")]
        public class MicrosoftData : SqlServerMiscellaneousTests
        {
            public MicrosoftData()
                : base(new SqlServerMSDataTestProvider())
            {
            }
        }
    }
}
