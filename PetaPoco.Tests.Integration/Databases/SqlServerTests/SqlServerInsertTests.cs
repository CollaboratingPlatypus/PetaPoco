using PetaPoco.Tests.Integration.Providers;
using Xunit;

namespace PetaPoco.Tests.Integration.Databases.SqlServer
{
    public abstract partial class SqlServerInsertTests : InsertTests
    {
        protected SqlServerInsertTests(TestProvider provider)
            : base(provider)
        {
        }

        [Collection("SqlServer.SystemData")]
        public class SystemData : SqlServerInsertTests
        {
            public SystemData()
                : base(new SqlServerSystemDataTestProvider())
            {
            }
        }

        [Collection("SqlServer.MicrosoftData")]
        public class MicrosoftData : SqlServerInsertTests
        {
            public MicrosoftData()
                : base(new SqlServerMSDataTestProvider())
            {
            }
        }
    }
}
