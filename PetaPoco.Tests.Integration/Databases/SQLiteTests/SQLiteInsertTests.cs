using PetaPoco.Tests.Integration.Providers;
using Xunit;

namespace PetaPoco.Tests.Integration.Databases.SQLite
{
    public abstract partial class SQLiteInsertTests : InsertTests
    {
        protected SQLiteInsertTests(TestProvider provider)
            : base(provider)
        {
        }

        [Collection("SQLite.SystemData")]
        public class SystemData : SQLiteInsertTests
        {
            public SystemData()
                : base(new SQLiteSystemDataTestProvider())
            {
            }
        }

        [Collection("SQLite.MicrosoftData")]
        public class MicrosoftData : SQLiteInsertTests
        {
            public MicrosoftData()
                : base(new SQLiteMSDataTestProvider())
            {
            }
        }
    }
}
