using System;
using System.Linq;

namespace PetaPoco.Tests.Integration.Databases.MSSQL
{
    public class MssqlDBTestProvider : BaseDbProviderFactory
    {
        private string _connectionName = "SqlServer";
        protected override string ConnectionName => _connectionName;

        protected override string ScriptResourceName => "PetaPoco.Tests.Integration.Scripts.SqlServerBuildDatabase.sql";

        public override IDatabase Execute()
        {
            EnsureDatabaseExists();
            return base.Execute();
        }

        public override void ExecuteBuildScript(IDatabase database, string script)
        {
            script.Split(new[] { "GO" }, StringSplitOptions.RemoveEmptyEntries).ToList().ForEach(s => { base.ExecuteBuildScript(database, s); });
        }

        private void EnsureDatabaseExists()
        {
            _connectionName = "SqlServer_Builder";
            Database.Execute("IF(db_id(N'PetaPoco') IS NULL) BEGIN CREATE DATABASE[PetaPoco] END");
            _connectionName = "SqlServer";
        }
    }
}
