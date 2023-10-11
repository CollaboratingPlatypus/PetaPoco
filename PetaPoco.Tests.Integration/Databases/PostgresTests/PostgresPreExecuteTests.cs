using System.Collections.Generic;
using System.Data;
using System.Linq;
using PetaPoco.Tests.Integration.Providers;
using Xunit;

namespace PetaPoco.Tests.Integration.Databases.Postgres
{
    [Collection("Postgres")]
    public class PostgresPreExecuteTests : PreExecuteTests
    {
        protected override IPreExecuteDatabaseProvider Provider => DB.Provider as PreExecuteDatabaseProvider;

        public PostgresPreExecuteTests()
            : base(new PreExecuteTestProvider())
        {
            Provider.ThrowExceptions = true;
        }

        protected class PreExecuteTestProvider : PostgresTestProvider
        {
            protected override IDatabase LoadFromConnectionName(string name)
                => BuildFromConnectionName(name).UsingProvider<PreExecuteDatabaseProvider>().Create();
        }

        protected class PreExecuteDatabaseProvider : PetaPoco.Providers.PostgreSQLDatabaseProvider, IPreExecuteDatabaseProvider
        {
            public bool ThrowExceptions { get; set; }
            public List<IDataParameter> Parameters { get; set; } = new List<IDataParameter>();

            public override void PreExecute(IDbCommand cmd)
            {
                Parameters.Clear();

                if (ThrowExceptions)
                {
                    Parameters = cmd.Parameters.Cast<IDataParameter>().ToList();
                    throw new PreExecuteException();
                }
            }
        }
    }
}
