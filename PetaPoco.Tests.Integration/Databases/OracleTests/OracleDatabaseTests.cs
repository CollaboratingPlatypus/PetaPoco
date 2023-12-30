using System.Reflection;
using PetaPoco.Tests.Integration.Providers;
using Xunit;

namespace PetaPoco.Tests.Integration.Databases.Oracle
{
    public abstract partial class OracleDatabaseTests : DatabaseTests
    {
        private OracleTestProvider _provider;

        protected OracleDatabaseTests(OracleTestProvider provider)
            : base(provider)
        {
            _provider = provider;
        }

        [Collection("Oracle.Delimited")]
        public class Delimited : OracleDatabaseTests
        {
            public Delimited()
                : base(new OracleDelimitedTestProvider())
            {
            }
        }

        [Collection("Oracle.Ordinary")]
        public class Ordinary : OracleDatabaseTests
        {
            public Ordinary()
                : base(new OracleOrdinaryTestProvider())
            {
            }

            /// <remarks>
            /// We need to retain the provider and mapper specified in the test provider
            /// to ensure correct logic is applied, because they might be custom implementations.
            /// </remarks>
            protected override void AfterDbCreate(Database db)
            {
                var thisDb = _provider.GetDatabase();
                base.AfterDbCreate(db);

                // ReSharper disable once PossibleNullReferenceException
                db.GetType().GetField("_provider", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(db, thisDb.Provider);
                // ReSharper disable once PossibleNullReferenceException
                db.GetType().GetField("_defaultMapper", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(db, thisDb.DefaultMapper);
            }
        }
    }
}
