using PetaPoco.Tests.Integration.Providers;
using Xunit;

namespace PetaPoco.Tests.Integration.Databases.SqlServer
{
    public abstract partial class SqlServerUpdateTests : UpdateTests
    {
        protected SqlServerUpdateTests(TestProvider provider)
            : base(provider)
        {
        }

        [Collection("SqlServer.SystemData")]
        public class SystemData : SqlServerUpdateTests
        {
            public SystemData()
                : base(new SqlServerSystemDataTestProvider())
            {
            }
        }

        [Collection("SqlServer.MicrosoftData")]
        public class MicrosoftData : SqlServerUpdateTests
        {
            public MicrosoftData()
                : base(new SqlServerMSDataTestProvider())
            {
            }
        }
    }
}
