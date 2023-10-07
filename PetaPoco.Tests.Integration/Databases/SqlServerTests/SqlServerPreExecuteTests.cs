using System.Collections.Generic;
using System.Data;
using System.Linq;
using PetaPoco.Providers;
using Xunit;

namespace PetaPoco.Tests.Integration.Databases.SqlServer
{
    public abstract partial class SqlServerPreExecuteTests : PreExecuteTests
    {
        protected SqlServerPreExecuteTests(BaseDbProviderFactory provider)
            : base(provider)
        { }

        [Collection("SqlServer.SystemData")]
        public class SystemData : SqlServerPreExecuteTests
        {
            protected override IExceptionDatabaseProvider Provider => DB.Provider as SqlServerSystemDataPreExecuteDatabaseProvider;

            public SystemData()
                : base(new SqlServerPreExecuteDbProviderFactory())
            {
                Provider.ThrowExceptions = true;
            }

            public class SqlServerPreExecuteDbProviderFactory : SqlServerSystemDataDbProviderFactory
            {
                protected override IDatabase LoadFromConnectionName(string name)
                {
                    var config = BuildFromConnectionName(name);
                    config.UsingProvider<SqlServerSystemDataPreExecuteDatabaseProvider>();
                    return config.Create();
                }
            }

            public class SqlServerSystemDataPreExecuteDatabaseProvider : SqlServerDatabaseProvider, IExceptionDatabaseProvider
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
            protected override IExceptionDatabaseProvider Provider => DB.Provider as SqlServerMSDataPreExecuteDatabaseProvider;

            public MicrosoftData()
                : base(new SqlServerPreExecuteDbProviderFactory())
            {
                Provider.ThrowExceptions = true;
            }

            public class SqlServerPreExecuteDbProviderFactory : SqlServerMSDataDbProviderFactory
            {
                protected override IDatabase LoadFromConnectionName(string name)
                {
                    var config = BuildFromConnectionName(name);
                    config.UsingProvider<SqlServerMSDataPreExecuteDatabaseProvider>();
                    return config.Create();
                }
            }

            public class SqlServerMSDataPreExecuteDatabaseProvider : SqlServerMsDataDatabaseProvider, IExceptionDatabaseProvider
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
