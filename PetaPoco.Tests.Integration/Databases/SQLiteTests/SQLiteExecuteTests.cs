using PetaPoco.Tests.Integration.Providers;
using Xunit;

namespace PetaPoco.Tests.Integration.Databases.SQLite
{
    public abstract partial class SQLiteExecuteTests : ExecuteTests
    {
        protected SQLiteExecuteTests(TestProvider provider)
            : base(provider)
        {
        }

        [Collection("SQLite.SystemData")]
        public class SystemData : SQLiteExecuteTests
        {
            public SystemData()
                : base(new SQLiteSystemDataTestProvider())
            {
            }
        }

        [Collection("SQLite.MicrosoftData")]
        public class MicrosoftData : SQLiteExecuteTests
        {
            public MicrosoftData()
                : base(new SQLiteMSDataTestProvider())
            {
            }
        }
    }
}
