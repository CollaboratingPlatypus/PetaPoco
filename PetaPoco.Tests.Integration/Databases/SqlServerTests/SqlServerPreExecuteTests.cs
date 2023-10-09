using System.Collections.Generic;
using System.Data;
using System.Linq;
using PetaPoco.Tests.Integration.Providers;
using Xunit;

namespace PetaPoco.Tests.Integration.Databases.SqlServer
{
    public abstract partial class SqlServerPreExecuteTests : PreExecuteTests
    {
        protected SqlServerPreExecuteTests(TestProvider provider)
            : base(provider)
        {
        }

        [Collection("SqlServer.SystemData")]
        public class SystemData : SqlServerPreExecuteTests
        {
            protected override IPreExecuteDatabaseProvider Provider => DB.Provider as PreExecuteDatabaseProvider;

            public SystemData()
                : base(new PreExecuteTestProvider())
            {
                Provider.ThrowExceptions = true;
            }

            protected class PreExecuteTestProvider : SqlServerSystemDataTestProvider
            {
                protected override IDatabase LoadFromConnectionName(string name)
                    => BuildFromConnectionName(name).UsingProvider<PreExecuteDatabaseProvider>().Create();
            }

            protected class PreExecuteDatabaseProvider : PetaPoco.Providers.SqlServerDatabaseProvider, IPreExecuteDatabaseProvider
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

        [Collection("SqlServer.MicrosoftData")]
        public class MicrosoftData : SqlServerPreExecuteTests
        {
            protected override IPreExecuteDatabaseProvider Provider => DB.Provider as PreExecuteDatabaseProvider;

            public MicrosoftData()
                : base(new PreExecuteTestProvider())
            {
                Provider.ThrowExceptions = true;
            }

            protected class PreExecuteTestProvider : SqlServerMSDataTestProvider
            {
                protected override IDatabase LoadFromConnectionName(string name)
                    => BuildFromConnectionName(name).UsingProvider<PreExecuteDatabaseProvider>().Create();
            }

            protected class PreExecuteDatabaseProvider : PetaPoco.Providers.SqlServerMsDataDatabaseProvider, IPreExecuteDatabaseProvider
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
}
