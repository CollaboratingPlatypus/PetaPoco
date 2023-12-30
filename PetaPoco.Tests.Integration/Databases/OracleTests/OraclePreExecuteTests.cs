using System.Collections.Generic;
using System.Data;
using System.Linq;
using PetaPoco.Tests.Integration.Providers;
using Xunit;

namespace PetaPoco.Tests.Integration.Databases.Oracle
{
    public abstract partial class OraclePreExecuteTests : PreExecuteTests
    {
        protected OraclePreExecuteTests(TestProvider provider)
            : base(provider)
        {
        }

        [Collection("Oracle.Delimited")]
        public class Delimited : OraclePreExecuteTests
        {
            protected override IPreExecuteDatabaseProvider Provider => DB.Provider as PreExecuteDatabaseProvider;

            public Delimited()
                : base(new PreExecuteTestProvider())
            {
                Provider.ThrowExceptions = true;
            }

            protected class PreExecuteTestProvider : OracleDelimitedTestProvider
            {
                protected override IDatabase LoadFromConnectionName(string name)
                    => BuildFromConnectionName(name).UsingProvider<PreExecuteDatabaseProvider>().Create();
            }

            protected class PreExecuteDatabaseProvider : PetaPoco.Providers.OracleDatabaseProvider, IPreExecuteDatabaseProvider
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

        [Collection("Oracle.Ordinary")]
        public class Ordinary : OraclePreExecuteTests
        {
            protected override IPreExecuteDatabaseProvider Provider => DB.Provider as PreExecuteDatabaseProvider;

            public Ordinary()
                : base(new PreExecuteTestProvider())
            {
                Provider.ThrowExceptions = true;
            }

            protected class PreExecuteTestProvider : OracleOrdinaryTestProvider
            {
                protected override IDatabase LoadFromConnectionName(string name)
                    => BuildFromConnectionName(name).UsingProvider<PreExecuteDatabaseProvider>().Create();
            }

            protected class PreExecuteDatabaseProvider : PetaPoco.Providers.OracleDatabaseProvider, IPreExecuteDatabaseProvider
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
