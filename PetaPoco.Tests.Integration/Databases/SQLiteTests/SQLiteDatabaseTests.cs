using System.Reflection;
using Xunit;

namespace PetaPoco.Tests.Integration.Databases.SQLite
{
    [Collection("SQLite")]
    public class SQLiteDatabaseTests : DatabaseTests
    {
        private readonly SQLiteDbProviderFactory _provider;

        public SQLiteDatabaseTests()
            : this(new SQLiteDbProviderFactory())
        {
        }

        private SQLiteDatabaseTests(SQLiteDbProviderFactory provider)
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
    }
}
