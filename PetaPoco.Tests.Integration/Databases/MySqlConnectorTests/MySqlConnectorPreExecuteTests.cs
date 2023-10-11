using System.Collections.Generic;
using System.Data;
using System.Linq;
using PetaPoco.Tests.Integration.Providers;
using Xunit;

namespace PetaPoco.Tests.Integration.Databases.MySqlConnector
{
    [Collection("MySqlConnector")]
    public class MySqlConnectorPreExecuteTests : PreExecuteTests
    {
        protected override IPreExecuteDatabaseProvider Provider => DB.Provider as PreExecuteDatabaseProvider;

        public MySqlConnectorPreExecuteTests()
            : base(new PreExecuteTestProvider())
        {
            Provider.ThrowExceptions = true;
        }

        protected class PreExecuteTestProvider : MySqlConnectorTestProvider
        {
            protected override IDatabase LoadFromConnectionName(string name)
                => BuildFromConnectionName(name).UsingProvider<PreExecuteDatabaseProvider>().Create();
        }

        protected class PreExecuteDatabaseProvider : PetaPoco.Providers.MySqlConnectorDatabaseProvider, IPreExecuteDatabaseProvider
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
