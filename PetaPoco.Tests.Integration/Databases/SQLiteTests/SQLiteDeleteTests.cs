using PetaPoco.Tests.Integration.Providers;
using Xunit;

namespace PetaPoco.Tests.Integration.Databases.SQLite
{
    public abstract partial class SQLiteDeleteTests : DeleteTests
    {
        protected SQLiteDeleteTests(TestProvider provider)
            : base(provider)
        {
        }

        [Collection("SQLite.SystemData")]
        public class SystemData : SQLiteDeleteTests
        {
            public SystemData()
                : base(new SQLiteSystemDataTestProvider())
            {
            }
        }

        [Collection("SQLite.MicrosoftData")]
        public class MicrosoftData : SQLiteDeleteTests
        {
            public MicrosoftData()
                : base(new SQLiteMSDataTestProvider())
            {
            }
        }
    }
}
