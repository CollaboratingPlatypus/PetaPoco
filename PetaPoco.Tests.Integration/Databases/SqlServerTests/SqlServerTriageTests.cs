using Xunit;

namespace PetaPoco.Tests.Integration.Databases.SqlServer
{
    public abstract partial class SqlServerTriageTests : TriageTests
    {
        protected SqlServerTriageTests(BaseDbProviderFactory provider)
            : base(provider)
        { }

        [Collection("SqlServer.SystemData")]
        public class SystemData : SqlServerTriageTests
        {
            public SystemData()
                : base(new SqlServerSystemDataDbProviderFactory())
            {
            }
        }

        [Collection("SqlServer.MicrosoftData")]
        public class MicrosoftData : SqlServerTriageTests
        {
            public MicrosoftData()
                : base(new SqlServerMSDataDbProviderFactory())
            {
            }
        }
    }
}
