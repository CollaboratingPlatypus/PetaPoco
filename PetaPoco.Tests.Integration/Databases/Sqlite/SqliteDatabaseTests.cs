using System.Reflection;
using Xunit;

namespace PetaPoco.Tests.Integration.Databases.Sqlite
{
    [Collection("Sqlite")]
    [Trait("Category", "Sqlite")]
    public class SqliteDatabaseTests : BaseDatabaseTests
    {
        private readonly SqliteDBTestProvider _dbTestProvider;

        public SqliteDatabaseTests()
            : this(new SqliteDBTestProvider())
        {
        }

        private SqliteDatabaseTests(SqliteDBTestProvider dbTestProvider)
            : base(dbTestProvider)
        {
            _dbTestProvider = dbTestProvider;
        }

        /// <remarks>
        ///     This is required because we can't use the Mapper.* methods, as we're testing many different databases and it would
        ///     apply Sqlite logic incorrectly.
        /// </remarks>
        protected override void AfterDbCreate(Database db)
        {
            base.AfterDbCreate(db);

            // ReSharper disable once PossibleNullReferenceException
            db.GetType().GetField("_defaultMapper", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(db, _dbTestProvider.GetDatabase().DefaultMapper);
        }
    }
}
