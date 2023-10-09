using PetaPoco.Tests.Integration.Providers;
using Xunit;

namespace PetaPoco.Tests.Integration.Databases.SQLite
{
    public abstract partial class SQLiteQueryLinqTests : QueryLinqTests
    {
        protected SQLiteQueryLinqTests(TestProvider provider)
            : base(provider)
        {
        }

        [Collection("SQLite.SystemData")]
        public class SystemData : SQLiteQueryLinqTests
        {
            public SystemData()
                : base(new SQLiteSystemDataTestProvider())
            {
            }
        }

        [Collection("SQLite.MicrosoftData")]
        public class MicrosoftData : SQLiteQueryLinqTests
        {
            public MicrosoftData()
                : base(new SQLiteMSDataTestProvider())
            {
            }
        }
    }
}
