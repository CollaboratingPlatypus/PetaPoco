using System.Reflection;
using PetaPoco.Tests.Integration.Providers;
using Xunit;

namespace PetaPoco.Tests.Integration.Databases.SQLite
{
    public abstract partial class SQLiteDatabaseTests : DatabaseTests
    {
        private readonly SQLiteTestProvider _provider;

        protected SQLiteDatabaseTests(SQLiteTestProvider provider)
            : base(provider)
        {
            _provider = provider;
        }

        /// <remarks>
        /// This is required because we can't use the Mapper.* methods, as we're testing many different databases and it would apply Sqlite
        /// logic incorrectly.
        /// </remarks>
        protected override void AfterDbCreate(Database db)
        {
            base.AfterDbCreate(db);

            // ReSharper disable once PossibleNullReferenceException
            db.GetType().GetField("_defaultMapper", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(db, _provider.GetDatabase().DefaultMapper);
        }

        [Collection("SQLite.SystemData")]
        public class SystemData : SQLiteDatabaseTests
        {
            public SystemData()
                : base(new SQLiteSystemDataTestProvider())
            {
            }
        }

        [Collection("SQLite.MicrosoftData")]
        public class MicrosoftData : SQLiteDatabaseTests
        {
            public MicrosoftData()
                : base(new SQLiteMSDataTestProvider())
            {
            }
        }
    }
}
