using System.Collections.Generic;
using System.Data;
using System.Linq;
using PetaPoco.Tests.Integration.Providers;
using Xunit;

namespace PetaPoco.Tests.Integration.Databases.SQLite
{
    [Collection("SQLite")]
    public abstract partial class SQLitePreExecuteTests : PreExecuteTests
    {
        protected SQLitePreExecuteTests(TestProvider provider)
            : base(provider)
        {
        }

        [Collection("SQLite.SystemData")]
        public class SystemData : SQLitePreExecuteTests
        {
            protected override IPreExecuteDatabaseProvider Provider => DB.Provider as PreExecuteDatabaseProvider;

            public SystemData()
                : base(new PreExecuteTestProvider())
            {
                Provider.ThrowExceptions = true;
            }

            protected class PreExecuteTestProvider : SQLiteSystemDataTestProvider
            {
                protected override IDatabase LoadFromConnectionName(string name)
                    => BuildFromConnectionName(name).UsingProvider<PreExecuteDatabaseProvider>().Create();
            }

            protected class PreExecuteDatabaseProvider : PetaPoco.Providers.SQLiteDatabaseProvider, IPreExecuteDatabaseProvider
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

        [Collection("SQLite.MicrosoftData")]
        public class MicrosoftData : SQLitePreExecuteTests
        {
            protected override IPreExecuteDatabaseProvider Provider => DB.Provider as PreExecuteDatabaseProvider;

            public MicrosoftData()
                : base(new PreExecuteTestProvider())
            {
                Provider.ThrowExceptions = true;
            }

            protected class PreExecuteTestProvider : SQLiteMSDataTestProvider
            {
                protected override IDatabase LoadFromConnectionName(string name)
                    => BuildFromConnectionName(name).UsingProvider<PreExecuteDatabaseProvider>().Create();
            }

            protected class PreExecuteDatabaseProvider : PetaPoco.Providers.SQLiteDatabaseProvider, IPreExecuteDatabaseProvider
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
